using App.Modules.Wikis.Domain.Domains.Wikis.Configuration.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using App.Modules.Sys.Infrastructure.Services.Contracts;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Services.BodyStorage.Implementations
{
    /// <summary>
    /// The <see cref="WikiBodyStorageSinkKind.FileSystem"/> body store (ADR-018N
    /// §2.2): writes a page-version body as a text file under a configured
    /// <b>external</b> wiki content repository, for the Phase-J
    /// documentation-as-source-code round-trip and Git-editable bodies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Never the module source tree.</b> The file root is
    /// <see cref="WikiConfigurationObject.BodyStorageFileSystemContentRepositoryRootPath"/>,
    /// which must be an absolute path outside application source (e.g. a dedicated
    /// wiki content Git working copy). If that root is unset or does not exist,
    /// this sink <b>fails loudly</b> — it logs and throws rather than ever writing
    /// into <c>App.Modules.Wikis</c> (ADR-018N §2.2, the house fail-first stance).
    /// </para>
    /// <para>
    /// <b>Locator.</b> The body locator is the content-repo-relative path produced
    /// by <see cref="WikiBodyPathFactory.BuildFileSystemRelativePath"/> from the
    /// wiki key, slug, and version number — human- and Git-friendly, and
    /// recomputable, so a page move never strands a stored path. The content hash
    /// is computed via <see cref="WikiBodyContentHasher"/> so it matches every
    /// other sink (ADR-018N §2.6).
    /// </para>
    /// <para>
    /// <b>Not transactional.</b> A file write is not part of the DB transaction;
    /// consistency between the version row and the file is eventual (ADR-018N
    /// §2.2). This sink is therefore typically a mirror behind a DB primary, not
    /// the sole authority, unless a deployment deliberately accepts that trade-off.
    /// </para>
    /// </remarks>
    public sealed class WikiFileSystemBodyStore : IWikiBodyStore
    {
        private readonly IAppConfiguration<WikiConfigurationObject> _configuration;
        private readonly IAppLogger _logger;

        /// <summary>
        /// Initialises a new <see cref="WikiFileSystemBodyStore"/>.
        /// </summary>
        /// <param name="configuration">Accessor for the wiki configuration object.</param>
        /// <param name="logger">Logger for diagnostics.</param>
        public WikiFileSystemBodyStore(
            IAppConfiguration<WikiConfigurationObject> configuration,
            IAppLogger logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        /// <inheritdoc />
        public WikiBodyStorageSinkKind Kind => WikiBodyStorageSinkKind.FileSystem;

        /// <inheritdoc />
        public async Task<WikiBodyStoreResult> StoreBodyAsync(
            WikiBodyStoreContext context,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            string repositoryRoot = this.ResolveContentRepositoryRootOrThrow();

            string relativePath = WikiBodyPathFactory.BuildFileSystemRelativePath(
                context.WikiKey,
                context.Slug,
                context.VersionNumber);

            string absolutePath = this.ResolveAbsolutePath(repositoryRoot, relativePath);

            string? directory = Path.GetDirectoryName(absolutePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(absolutePath, body.ToArray(), cancellationToken)
                .ConfigureAwait(false);

            string contentHash = WikiBodyContentHasher.ComputeHash(body.Span);

            // The locator is the content-repo-relative path, recomputable from the
            // version's stable facts, so a page move never strands it.
            return new WikiBodyStoreResult(relativePath, contentHash, body.Length);
        }

        /// <inheritdoc />
        public async Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
            WikiBodyStoreContext context,
            string bodyLocator,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentException.ThrowIfNullOrWhiteSpace(bodyLocator);

            string repositoryRoot = this.ResolveContentRepositoryRootOrThrow();
            string absolutePath = this.ResolveAbsolutePath(repositoryRoot, bodyLocator);

            if (!File.Exists(absolutePath))
            {
                // Missing file is an explicit diagnostic, never a silent fallback
                // (ADR-018 §1.2). The coordinator reads the primary only.
                this._logger.LogWarning(
                    "WikiFileSystemBodyStore could not find body file '{0}' for page {1}.",
                    bodyLocator,
                    context.WikiPageId);
                return null;
            }

            byte[] bytes = await File.ReadAllBytesAsync(absolutePath, cancellationToken)
                .ConfigureAwait(false);
            return bytes;
        }

        /// <summary>
        /// Resolves the configured content-repository root, failing loudly when it
        /// is unset or missing so we never write into the module source tree.
        /// </summary>
        private string ResolveContentRepositoryRootOrThrow()
        {
            WikiConfigurationObject configuration = this._configuration.GetValueOrDefault();
            string? root = configuration.BodyStorageFileSystemContentRepositoryRootPath?.Trim();

            if (string.IsNullOrWhiteSpace(root))
            {
                string message =
                    "The FileSystem wiki body sink is selected but no external content-repository root is configured "
                    + "(BodyStorageFileSystemContentRepositoryRootPath). Refusing to write wiki bodies into application source.";
                this._logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            if (!Directory.Exists(root))
            {
                string message =
                    $"The configured wiki body content-repository root '{root}' does not exist. "
                    + "Refusing to write wiki bodies into application source.";
                this._logger.LogError(message);
                throw new DirectoryNotFoundException(message);
            }

            return root;
        }

        /// <summary>
        /// Combines the content-repo root with a repo-relative locator path,
        /// normalising the <c>/</c> separators used by the path factory to the
        /// platform separator, and guarding against path traversal escaping the
        /// configured root.
        /// </summary>
        private string ResolveAbsolutePath(string repositoryRoot, string relativePath)
        {
            string platformRelative = relativePath
                .Replace(WikiDomainConstants.PathSegmentSeparator, Path.DirectorySeparatorChar.ToString());

            string combined = Path.GetFullPath(Path.Combine(repositoryRoot, platformRelative));
            string normalizedRoot = Path.GetFullPath(repositoryRoot);

            // Defence in depth: a locator must never resolve outside the root.
            if (!combined.StartsWith(normalizedRoot, StringComparison.Ordinal))
            {
                string message =
                    $"The wiki body locator '{relativePath}' resolves outside the configured content-repository root.";
                this._logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            return combined;
        }
    }
}
