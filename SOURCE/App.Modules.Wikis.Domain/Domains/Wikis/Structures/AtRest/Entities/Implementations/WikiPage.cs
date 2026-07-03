using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// A <c>WikiPage</c> is the stable identity of a page within a
    /// <see cref="Wiki"/>. It is deliberately thin: it owns the addressing
    /// (the canonical <see cref="Path"/>) and points at the current published
    /// version, but it carries <b>no body text</b>. All body content is
    /// immutable and lives in <see cref="WikiPageVersion"/> rows (ADR-018
    /// immutable-blob invariant), so "editing a page" never mutates this row's
    /// content — it appends a new version and repoints
    /// <see cref="CurrentVersionId"/>.
    /// <para>
    /// Addressing is DokuWiki-style: <see cref="Path"/> is the source of truth
    /// (prefix = namespace, last segment = leaf). <see cref="Slug"/> is the
    /// derived leaf and <see cref="ParentWikiPageFK"/> is an optional,
    /// non-authoritative explicit parent link — ancestry and children are
    /// derived from the path.
    /// </para>
    /// </summary>
    public class WikiPage : DefaultEntityBase, IHasTitleAndDescription, IHasEnabled
    {
        /// <summary>
        /// FK to the owning <see cref="Wiki"/> root.
        /// <para>Navigable, so the suffix is <c>FK</c>, not <c>Id</c>.</para>
        /// </summary>
        public Guid WikiFK { get; set; }

        /// <summary>
        /// Optional FK to a parent <see cref="WikiPage"/>.
        /// <para>
        /// <b>Non-authoritative for addressing.</b> Ancestry is derived from
        /// <see cref="Path"/> (split on <c>/</c>); this column is retained only
        /// as an optional explicit parent link and is <c>null</c> for a
        /// top-level page. A page whose path-parent does not yet exist is valid
        /// (DokuWiki-style): navigating there yields a blank page that is an
        /// invitation to create it.
        /// </para>
        /// </summary>
        public Guid? ParentWikiPageFK { get; set; }

        /// <summary>
        /// The canonical, full namespace path of this page within its
        /// <see cref="Wiki"/> (e.g. <c>engineering/onboarding/setup</c>),
        /// unique within the wiki root.
        /// <para>
        /// This is the <b>single source of truth for addressing</b>
        /// (DokuWiki-style): the prefix up to the last <c>/</c> is the
        /// "namespace" and the final segment is the leaf. There is no separate
        /// namespace entity. <see cref="Slug"/> is merely the derived last
        /// segment of this path.
        /// </para>
        /// <para>
        /// <c>Path</c> is canonical and may itself be non-URL-safe; the render
        /// layer is responsible for URL-mangling it for safe links and copy.
        /// Full-text/prefix search runs against this field.
        /// </para>
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// The derived last segment (leaf) of <see cref="Path"/>, unique within
        /// its <see cref="Wiki"/>. Retained for routing and
        /// <c>wiki:{key}:{slug}</c> cross-links; it is computed from
        /// <see cref="Path"/> and is not an independent authority.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <summary>
        /// FK to the <see cref="WikiPageVersion"/> that is currently published
        /// (rendered to readers). <c>null</c> for a freshly-created page that
        /// has no published version yet.
        /// </summary>
        public Guid? CurrentVersionId { get; set; }

        /// <summary>
        /// Navigation: the owning wiki root.
        /// </summary>
        public Wiki? Wiki { get; set; }

        /// <summary>
        /// Navigation: the parent page (when this page is nested).
        /// </summary>
        public WikiPage? Parent { get; set; }

        /// <summary>
        /// Navigation: child pages nested under this page.
        /// </summary>
        public ICollection<WikiPage> Children { get; set; } = new List<WikiPage>();

        /// <summary>
        /// Navigation: the immutable version history for this page.
        /// </summary>
        public ICollection<WikiPageVersion> Versions { get; set; } = new List<WikiPageVersion>();

        /// <summary>
        /// Navigation: the immutable media handles attached to this page.
        /// </summary>
        public ICollection<WikiMedia> Media { get; set; } = new List<WikiMedia>();

        /// <summary>
        /// Navigation: page-scoped access-control grants overriding the wiki-wide grants.
        /// </summary>
        public ICollection<WikiAcl> Acls { get; set; } = new List<WikiAcl>();
    }
}
