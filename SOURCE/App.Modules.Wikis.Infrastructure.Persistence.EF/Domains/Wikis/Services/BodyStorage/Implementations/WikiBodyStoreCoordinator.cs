using App.Modules.Wikis.Domain.Domains.Wikis.Configuration.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using App.Modules.Sys.Infrastructure.Services.Contracts;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Services.BodyStorage.Implementations
{
    /// <summary>
    /// Default <see cref="IWikiBodyStoreCoordinator"/> (ADR-018N §2.3/§2.4): the
    /// single entry point the page-save use case calls, dispatching to the
    /// configured primary sink and fanning writes out to configured mirror sinks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Write:</b> the body is written to the configured primary sink (its
    /// result — and therefore its locator — is what the version row persists),
    /// then best-effort to each configured mirror sink. A mirror failure is logged
    /// and tolerated unless
    /// <see cref="WikiConfigurationObject.BodyStorageFailIfMirrorSinkUnavailable"/>
    /// is set, so a missing content repo never blocks authoring by default.
    /// </para>
    /// <para>
    /// <b>Read:</b> always from the primary sink only. Mirrors are write-only
    /// durability/round-trip targets; reading the primary keeps read semantics
    /// single-sourced (ADR-018N §2.4).
    /// </para>
    /// <para>
    /// Sinks are discovered as the injected <see cref="IWikiBodyStore"/> set and
    /// indexed by <see cref="IWikiBodyStore.Kind"/>, so adding a sink never
    /// touches this coordinator.
    /// </para>
    /// </remarks>
    public sealed class WikiBodyStoreCoordinator : IWikiBodyStoreCoordinator
    {
        private readonly Dictionary<WikiBodyStorageSinkKind, IWikiBodyStore> _sinksByKind;
        private readonly IAppConfiguration<WikiConfigurationObject> _configuration;
        private readonly IAppLogger _logger;

        /// <summary>
        /// Initialises a new <see cref="WikiBodyStoreCoordinator"/>.
        /// </summary>
        /// <param name="bodyStores">All discovered single-sink body stores.</param>
        /// <param name="configuration">Accessor for the wiki configuration object.</param>
        /// <param name="logger">Logger for diagnostics.</param>
        public WikiBodyStoreCoordinator(
            IEnumerable<IWikiBodyStore> bodyStores,
            IAppConfiguration<WikiConfigurationObject> configuration,
            IAppLogger logger)
        {
            ArgumentNullException.ThrowIfNull(bodyStores);

            // One store per kind; a duplicate registration is a wiring fault.
            Dictionary<WikiBodyStorageSinkKind, IWikiBodyStore> map = new Dictionary<WikiBodyStorageSinkKind, IWikiBodyStore>();
            foreach (IWikiBodyStore store in bodyStores)
            {
                map[store.Kind] = store;
            }

            this._sinksByKind = map;
            this._configuration = configuration;
            this._logger = logger;
        }

        /// <inheritdoc />
        public async Task<WikiBodyStoreResult> StoreBodyAsync(
            WikiBodyStoreContext context,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            WikiConfigurationObject configuration = this._configuration.GetValueOrDefault();

            IWikiBodyStore primary = this.ResolveSinkOrThrow(configuration.BodyStoragePrimarySink, isPrimary: true);

            WikiBodyStoreResult result = await primary
                .StoreBodyAsync(context, body, cancellationToken)
                .ConfigureAwait(false);

            await this.MirrorAsync(context, body, configuration, primary.Kind, cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
            WikiBodyStoreContext context,
            string bodyLocator,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            WikiConfigurationObject configuration = this._configuration.GetValueOrDefault();
            IWikiBodyStore primary = this.ResolveSinkOrThrow(configuration.BodyStoragePrimarySink, isPrimary: true);

            return primary.GetBodyBytesAsync(context, bodyLocator, cancellationToken);
        }

        /// <summary>
        /// Writes the body to each configured mirror sink (other than the primary),
        /// best-effort by default. A mirror failure is logged; it only propagates
        /// when the deployment opts in via
        /// <see cref="WikiConfigurationObject.BodyStorageFailIfMirrorSinkUnavailable"/>.
        /// </summary>
        private async Task MirrorAsync(
            WikiBodyStoreContext context,
            ReadOnlyMemory<byte> body,
            WikiConfigurationObject configuration,
            WikiBodyStorageSinkKind primaryKind,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<WikiBodyStorageSinkKind> mirrorKinds =
                configuration.BodyStorageMirrorSinks ?? Array.Empty<WikiBodyStorageSinkKind>();

            foreach (WikiBodyStorageSinkKind mirrorKind in mirrorKinds)
            {
                // Skip the primary (already written) and any meaningless sentinel.
                if (mirrorKind == primaryKind
                    || mirrorKind == WikiBodyStorageSinkKind.Undefined
                    || mirrorKind == WikiBodyStorageSinkKind.NotApplicable
                    || mirrorKind == WikiBodyStorageSinkKind.Unspecified
                    || mirrorKind == WikiBodyStorageSinkKind.Unknown)
                {
                    continue;
                }

                if (!this._sinksByKind.TryGetValue(mirrorKind, out IWikiBodyStore? mirror))
                {
                    this.HandleMirrorProblem(
                        configuration,
                        $"No wiki body store is registered for configured mirror sink '{mirrorKind}'.",
                        exception: null);
                    continue;
                }

                try
                {
                    await mirror.StoreBodyAsync(context, body, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    this.HandleMirrorProblem(
                        configuration,
                        $"Failed to mirror wiki body to sink '{mirrorKind}' for page {context.WikiPageId}.",
                        exception);
                }
            }
        }

        /// <summary>
        /// Logs a mirror problem, and rethrows it when the deployment requires
        /// mirror writes to be mandatory.
        /// </summary>
        private void HandleMirrorProblem(
            WikiConfigurationObject configuration,
            string message,
            Exception? exception)
        {
            if (exception is null)
            {
                this._logger.LogWarning(message);
            }
            else
            {
                this._logger.LogWarning(exception, message);
            }

            if (configuration.BodyStorageFailIfMirrorSinkUnavailable)
            {
                throw new InvalidOperationException(message, exception);
            }
        }

        /// <summary>
        /// Resolves the body store for a sink kind, throwing when no store is
        /// registered for a required sink (a wiring/configuration fault).
        /// </summary>
        private IWikiBodyStore ResolveSinkOrThrow(WikiBodyStorageSinkKind kind, bool isPrimary)
        {
            if (this._sinksByKind.TryGetValue(kind, out IWikiBodyStore? store))
            {
                return store;
            }

            string role = isPrimary ? "primary" : "mirror";
            string message =
                $"No wiki body store is registered for the configured {role} sink '{kind}'.";
            this._logger.LogError(message);
            throw new InvalidOperationException(message);
        }
    }
}
