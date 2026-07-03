namespace App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage
{
    /// <summary>
    /// The post-authorisation addressing facts a <see cref="IWikiBodyStore"/>
    /// needs to store or retrieve a single page-version body (ADR-018N §2.1).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This carries <em>no principal</em>: authorisation is enforced upstream by
    /// the application layer (ADR-018 §2.3), exactly as for media. The body store
    /// is byte I/O that runs only after the caller has been permitted, so a
    /// context object never re-opens the access question.
    /// </para>
    /// <para>
    /// The fields are the stable identity of the version plus the few facts a
    /// sink may legitimately use to choose a human-readable storage path
    /// (<c>WikiKey</c>, <c>Slug</c>, <c>VersionNumber</c>) or container tier
    /// (<c>AccessTier</c>). A sink must treat them as advisory for path
    /// readability only — the authoritative handle is always the returned
    /// <see cref="WikiBodyStoreResult.BodyLocator"/>.
    /// </para>
    /// </remarks>
    public sealed class WikiBodyStoreContext
    {
        /// <summary>The owning <c>Wiki</c> root id.</summary>
        public Guid WikiId { get; init; }

        /// <summary>
        /// The owning wiki's slug-addressable mount key (e.g. <c>docs</c>). Used
        /// only by sinks that build a human-readable path; never an authority.
        /// </summary>
        public string WikiKey { get; init; } = string.Empty;

        /// <summary>The owning <c>WikiPage</c> id.</summary>
        public Guid WikiPageId { get; init; }

        /// <summary>
        /// The page's namespace path relative to the wiki root (e.g.
        /// <c>a/b/c/d</c>). Path-building sinks may incorporate it; it is never
        /// the body's authority (the locator is).
        /// </summary>
        public string Slug { get; init; } = string.Empty;

        /// <summary>The immutable <c>WikiPageVersion</c> id this body belongs to.</summary>
        public Guid WikiPageVersionId { get; init; }

        /// <summary>The 1-based version number within the page.</summary>
        public int VersionNumber { get; init; }

        /// <summary>
        /// The declared content-format key of the body (ADR-018E), e.g.
        /// <c>markdown</c>. Advisory for path extension/readability only.
        /// </summary>
        public string ContentFormatKey { get; init; } = string.Empty;

        /// <summary>
        /// Whether the owning wiki is private (signed-access) or public. A sink
        /// that targets the object store uses this to choose its container tier,
        /// consistent with the wiki media model.
        /// </summary>
        public bool IsPrivate { get; init; } = true;
    }
}
