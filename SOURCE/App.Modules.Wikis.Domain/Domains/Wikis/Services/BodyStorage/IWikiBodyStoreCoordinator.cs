using App.Modules.Sys.Shared.Services;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage
{
    /// <summary>
    /// The single entry point the page-save use case calls to persist or read a
    /// wiki page-version body, hiding the configured sink selection and mirror
    /// fan-out behind one call (ADR-018N §2.3/§2.4).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Write</b> goes to the configured primary sink and then, best-effort, to
    /// each configured mirror sink (e.g. a DB-authoritative body mirrored to a
    /// file content repo). Mirror failures are logged and tolerated by default so
    /// a missing content repo never blocks authoring; an administrator may opt to
    /// fail the save instead (ADR-018N §2.3).
    /// </para>
    /// <para>
    /// <b>Read</b> always uses the primary sink only. Mirrors are write-only
    /// durability/round-trip targets, never read, to keep read semantics single
    /// sourced and predictable (ADR-018N §2.4). A missing primary body is an
    /// explicit diagnostic, never a silent fall-through to a mirror.
    /// </para>
    /// <para>
    /// This is the seam that lets the DB-vs-blob-vs-file decision be deferred to
    /// configuration without the caller (or stored content) knowing which sink is
    /// active — the body is always addressed by the returned locator, persisted
    /// on the version row (ADR-018N §2.6).
    /// </para>
    /// <para>
    /// Scoped (<see cref="IHasScopedService"/>): it orchestrates the scoped
    /// <see cref="IWikiBodyStore"/> family within the page-save use-case scope.
    /// </para>
    /// </remarks>
    public interface IWikiBodyStoreCoordinator : IHasScopedService
    {
        /// <summary>
        /// Stores a version body to the configured primary sink and mirrors it to
        /// any configured mirror sinks (best-effort by default).
        /// </summary>
        /// <param name="context">The post-authorisation addressing facts for the body.</param>
        /// <param name="body">The raw body bytes to store.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The result from the <em>primary</em> sink (its locator, hash, and
        /// length); the primary locator is what the version row persists.
        /// </returns>
        Task<WikiBodyStoreResult> StoreBodyAsync(
            WikiBodyStoreContext context,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads a version body from the configured primary sink using the
        /// locator persisted on the version row.
        /// </summary>
        /// <param name="context">The addressing facts for the body.</param>
        /// <param name="bodyLocator">The locator persisted on the version row.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The body bytes, or <c>null</c> when the primary sink has no body for
        /// the locator (the caller raises an explicit diagnostic).
        /// </returns>
        Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
            WikiBodyStoreContext context,
            string bodyLocator,
            CancellationToken cancellationToken = default);
    }
}
