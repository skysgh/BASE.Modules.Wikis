using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using App.Modules.Wikis.Infrastructure.Persistence.EF;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Services.BodyStorage.Implementations
{
    /// <summary>
    /// The <see cref="WikiBodyStorageSinkKind.Database"/> body store (ADR-018N
    /// §2.2): persists a page-version body as a <see cref="WikiPageVersionBody"/>
    /// satellite row, transactional with the version and full-text indexable in
    /// place (ADR-018 §3.5 / feature F23).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Locator.</b> The body row is keyed 1:1 by the version id, so this sink's
    /// <see cref="WikiBodyStoreResult.BodyLocator"/> is simply the version id
    /// (string form). On read, the locator is parsed back to the version id and
    /// the body row fetched by its FK. This keeps the locator identical to the
    /// version's <c>BodyBlobId</c> under the Database sink (ADR-018N §2.6), so the
    /// coordinator resolves the body straight from the stored locator.
    /// </para>
    /// <para>
    /// <b>Late save.</b> Like the rest of the module, this sink does not call
    /// <c>SaveChanges</c> itself — it adds the body row to the context and lets the
    /// shared save pipeline persist it within the page-save transaction (house
    /// late-save pattern), so a body row and its version commit atomically.
    /// </para>
    /// <para>
    /// <b>Immutability.</b> A new version gets a new body row; this sink never
    /// updates an existing body in place.
    /// </para>
    /// </remarks>
    public sealed class WikiDatabaseBodyStore : IWikiBodyStore
    {
        private readonly ModuleDbContext _db;
        private readonly IAppLogger _logger;

        /// <summary>
        /// Initialises a new <see cref="WikiDatabaseBodyStore"/>.
        /// </summary>
        /// <param name="db">The module database context.</param>
        /// <param name="logger">Logger for diagnostics.</param>
        public WikiDatabaseBodyStore(ModuleDbContext db, IAppLogger logger)
        {
            this._db = db;
            this._logger = logger;
        }

        /// <inheritdoc />
        public WikiBodyStorageSinkKind Kind => WikiBodyStorageSinkKind.Database;

        /// <inheritdoc />
        public Task<WikiBodyStoreResult> StoreBodyAsync(
            WikiBodyStoreContext context,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.WikiPageVersionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "A database body row requires the owning version id.",
                    nameof(context));
            }

            // Decode the body to text: the Database sink stores searchable text,
            // not opaque bytes, which is the whole reason this sink exists.
            string bodyText = System.Text.Encoding.UTF8.GetString(body.Span);
            string contentHash = WikiBodyContentHasher.ComputeHash(body.Span);

            WikiPageVersionBody row = new WikiPageVersionBody
            {
                WikiPageVersionFK = context.WikiPageVersionId,
                Body = bodyText,
            };

            // Late save: stage the row; the shared pipeline commits it with the
            // version inside the same transaction.
            this._db.WikiPageVersionBodies.Add(row);

            // The locator is the version id (the body row's FK), so it equals the
            // version's BodyBlobId under this sink.
            string locator = context.WikiPageVersionId.ToString("D");

            return Task.FromResult(
                new WikiBodyStoreResult(locator, contentHash, body.Length));
        }

        /// <inheritdoc />
        public async Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
            WikiBodyStoreContext context,
            string bodyLocator,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentException.ThrowIfNullOrWhiteSpace(bodyLocator);

            if (!Guid.TryParse(bodyLocator, out Guid versionId))
            {
                // A malformed locator is a data/logic fault, not a missing body.
                this._logger.LogWarning(
                    "WikiDatabaseBodyStore received a non-GUID body locator '{0}' for page {1}.",
                    bodyLocator,
                    context.WikiPageId);
                return null;
            }

            WikiPageVersionBody? row = await this._db.WikiPageVersionBodies
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.WikiPageVersionFK == versionId, cancellationToken)
                .ConfigureAwait(false);

            if (row is null)
            {
                return null;
            }

            return System.Text.Encoding.UTF8.GetBytes(row.Body);
        }
    }
}
