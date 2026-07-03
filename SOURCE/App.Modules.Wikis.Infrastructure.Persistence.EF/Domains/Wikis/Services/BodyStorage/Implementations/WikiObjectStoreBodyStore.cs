using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.ObjectStorage.Models.Enums;
using App.Modules.Sys.Shared.ObjectStorage.Services;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Services.BodyStorage.Implementations
{
    /// <summary>
    /// The <see cref="WikiBodyStorageSinkKind.ObjectStore"/> body store (ADR-018N
    /// §2.2): persists a page-version body as an immutable object-store blob,
    /// keeping body bytes out of the relational backup for large deployments.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why direct object storage, not the media pipeline.</b> A body is authored
    /// text, not a scanned/resized media asset; routing it through the image/media
    /// upload pipeline would impose irrelevant scan/resize semantics. This sink
    /// therefore writes straight through <see cref="IObjectStorageService"/> (the
    /// same provider abstraction the media pipeline ultimately uses) and computes
    /// the content hash itself via <see cref="WikiBodyContentHasher"/>, so the hash
    /// is identical to every other sink's (ADR-018N §2.6).
    /// </para>
    /// <para>
    /// <b>Locator.</b> A fresh GUID is allocated as the body locator; the
    /// deterministic blob path is derived from it by
    /// <see cref="WikiBodyPathFactory.BuildObjectStorePath"/> (under
    /// <see cref="WikiDomainConstants.BodyBlobPathPrefix"/>, distinct from media),
    /// so the locator — not a stored path — is what the version row persists.
    /// </para>
    /// <para>
    /// <b>Container.</b> Bodies use the private (signed-access) container, matching
    /// the wiki media model: wiki content is ACL-gated, never anonymously listable.
    /// </para>
    /// </remarks>
    public sealed class WikiObjectStoreBodyStore : IWikiBodyStore
    {
        private readonly IObjectStorageService _objectStorageService;
        private readonly IAppLogger _logger;

        /// <summary>
        /// The container wiki bodies are stored in. Wiki content is ACL-gated, so
        /// it lives in the private (signed-access) container.
        /// </summary>
        private const StorageContainerType BodyContainer = StorageContainerType.Private;

        /// <summary>
        /// Initialises a new <see cref="WikiObjectStoreBodyStore"/>.
        /// </summary>
        /// <param name="objectStorageService">The object-store provider abstraction.</param>
        /// <param name="logger">Logger for diagnostics.</param>
        public WikiObjectStoreBodyStore(
            IObjectStorageService objectStorageService,
            IAppLogger logger)
        {
            this._objectStorageService = objectStorageService;
            this._logger = logger;
        }

        /// <inheritdoc />
        public WikiBodyStorageSinkKind Kind => WikiBodyStorageSinkKind.ObjectStore;

        /// <inheritdoc />
        public async Task<WikiBodyStoreResult> StoreBodyAsync(
            WikiBodyStoreContext context,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Identity is owned by the store: allocate a fresh immutable locator
            // and derive the deterministic blob path from it (never stored).
            Guid locator = Guid.NewGuid();
            string blobPath = WikiBodyPathFactory.BuildObjectStorePath(locator);
            string contentHash = WikiBodyContentHasher.ComputeHash(body.Span);

            using MemoryStream content = new MemoryStream(body.ToArray(), writable: false);

            await this._objectStorageService
                .UploadAsync(
                    BodyContainer,
                    blobPath,
                    content,
                    WikiDomainConstants.BodyObjectStoreMediaType,
                    metadata: null,
                    cancellationToken)
                .ConfigureAwait(false);

            return new WikiBodyStoreResult(locator.ToString("D"), contentHash, body.Length);
        }

        /// <inheritdoc />
        public async Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
            WikiBodyStoreContext context,
            string bodyLocator,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentException.ThrowIfNullOrWhiteSpace(bodyLocator);

            if (!Guid.TryParse(bodyLocator, out Guid locator))
            {
                this._logger.LogWarning(
                    "WikiObjectStoreBodyStore received a non-GUID body locator '{0}' for page {1}.",
                    bodyLocator,
                    context.WikiPageId);
                return null;
            }

            string blobPath = WikiBodyPathFactory.BuildObjectStorePath(locator);

            bool exists = await this._objectStorageService
                .ExistsAsync(BodyContainer, blobPath, cancellationToken)
                .ConfigureAwait(false);

            if (!exists)
            {
                return null;
            }

            await using Stream stream = await this._objectStorageService
                .DownloadAsync(BodyContainer, blobPath, cancellationToken)
                .ConfigureAwait(false);

            using MemoryStream buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);
            return buffer.ToArray();
        }
    }
}
