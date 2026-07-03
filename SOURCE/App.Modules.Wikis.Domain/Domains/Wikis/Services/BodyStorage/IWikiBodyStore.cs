using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Sys.Shared.Services;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage
{
    /// <summary>
    /// A single, narrow seam owning the I/O of a wiki page-version <em>body</em>
    /// for one storage sink (ADR-018N §2.1). One implementation exists per
    /// <see cref="WikiBodyStorageSinkKind"/>; the
    /// <see cref="IWikiBodyStoreCoordinator"/> selects and composes them per the
    /// administrator's configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A body store knows <em>nothing</em> about the page tree, ACLs, or
    /// rendering — those are resolved before the body is fetched (ADR-018 §2.3),
    /// exactly as today for media. It is post-authorisation byte I/O.
    /// </para>
    /// <para>
    /// <b>Immutability.</b> Every implementation preserves the ADR-018
    /// invariant: a body write always yields a <em>new</em>
    /// <see cref="WikiBodyStoreResult.BodyLocator"/>, never an in-place
    /// mutation. "Editing" a page is appending a new version with a new body, so
    /// a store is never asked to overwrite.
    /// </para>
    /// <para>
    /// Extends <see cref="IHasScopedService"/> for convention-based, scoped DI
    /// discovery: the database sink is bound to a DbContext/unit-of-work and the
    /// whole family runs within the page-save use-case scope, so the stores are
    /// scoped rather than singleton. (It is deliberately the bare scoped marker
    /// rather than <c>IHasRepository</c> — a body store is not an entity
    /// repository — nor the cross-module <c>IHasDataStore</c> — it is internal to
    /// the Wikis module.)
    /// </para>
    /// </remarks>
    public interface IWikiBodyStore : IHasScopedService
    {
        /// <summary>
        /// The sink this store implements. The coordinator dispatches by this
        /// value, so each concrete store returns exactly one kind and no two
        /// stores share a kind.
        /// </summary>
        WikiBodyStorageSinkKind Kind { get; }

        /// <summary>
        /// Stores a version body and returns its sink-specific locator, hash, and
        /// length. Always allocates a new locator (immutability); never mutates
        /// an existing one.
        /// </summary>
        /// <param name="context">The post-authorisation addressing facts for the body.</param>
        /// <param name="body">The raw body bytes to store.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The locator, content hash, and byte length of the stored body.</returns>
        Task<WikiBodyStoreResult> StoreBodyAsync(
            WikiBodyStoreContext context,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the raw bytes of a previously stored body by the locator
        /// this same sink produced.
        /// </summary>
        /// <param name="context">The addressing facts for the body (the version it belongs to).</param>
        /// <param name="bodyLocator">The sink-specific locator from a prior <see cref="StoreBodyAsync"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The body bytes, or <c>null</c> when the locator resolves to nothing in
        /// this sink (e.g. a file-system body file is missing) — the caller then
        /// raises an explicit diagnostic rather than silently substituting, per
        /// ADR-018 §1.2.
        /// </returns>
        Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
            WikiBodyStoreContext context,
            string bodyLocator,
            CancellationToken cancellationToken = default);
    }
}
