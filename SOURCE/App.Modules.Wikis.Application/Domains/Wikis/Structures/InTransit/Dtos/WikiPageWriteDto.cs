using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Write DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPage"/>.
    /// Used for POST (create) and PUT (update) operations and serves as the
    /// structural base for <see cref="WikiPageReadDto"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A page is the stable identity of content within a wiki: it owns the
    /// addressing (slug + optional parent for a tree) and points at the current
    /// published version, but carries no body text. Body content is immutable
    /// and lives in version rows.
    /// </para>
    /// <para>
    /// FK naming rule: navigable relationships use the <c>FK</c> suffix
    /// (<c>WikiFK</c>, <c>ParentWikiPageFK</c>); <c>CurrentVersionId</c> uses the
    /// <c>Id</c> suffix as a deliberate non-navigable forward reference.
    /// </para>
    /// </remarks>
    public class WikiPageWriteDto : IHasGuidId, IHasTitleAndDescription, IHasEnabled
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the FK to the owning wiki root.
        /// </summary>
        public Guid WikiFK { get; set; }

        /// <summary>
        /// Gets or sets the optional FK to a parent page. <c>null</c> for a
        /// top-level page. Non-authoritative for addressing: ancestry is derived
        /// from <see cref="Path"/>; this link is retained as an optional
        /// explicit parent reference only.
        /// </summary>
        public Guid? ParentWikiPageFK { get; set; }

        /// <summary>
        /// Gets or sets the canonical full namespace path of this page within
        /// its wiki (e.g. <c>engineering/onboarding/setup</c>), unique within
        /// the wiki root. This is the DokuWiki-style addressing source of truth:
        /// the prefix up to the last <c>/</c> is the namespace and the final
        /// segment is the leaf. <see cref="Slug"/> is the derived leaf.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the derived last segment (leaf) of <see cref="Path"/>,
        /// unique only as a lookup aid. Retained for routing and cross-links.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the FK to the currently published version. <c>null</c>
        /// for a freshly-created page that has no published version yet.
        /// </summary>
        public Guid? CurrentVersionId { get; set; }
    }
}
