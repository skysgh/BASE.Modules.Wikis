namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// The write counterpart of <see cref="WikiPageContentReadDto"/>: the
    /// render-ready body the editor produces, addressed by the owning wiki root
    /// and the canonical DokuWiki-style <see cref="Path"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is what an "edit and save" submits. The server orchestration
    /// (store body bytes through the body store → append an immutable
    /// <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPageVersion"/>
    /// → repoint
    /// <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPage.CurrentVersionId"/>)
    /// never mutates an existing version in place (ADR-018 immutability
    /// invariant); it appends a new version and moves the page's current
    /// pointer.
    /// </para>
    /// <para>
    /// Addressing is by <see cref="WikiFK"/> + <see cref="Path"/> so a save can
    /// also <i>create</i> a previously-missing page (DokuWiki-style "create this
    /// page"): when no page exists at the path it is created, then its first
    /// version is appended.
    /// </para>
    /// </remarks>
    public class WikiPageContentWriteDto
    {
        /// <summary>
        /// Gets or sets the FK to the owning wiki root the page lives under.
        /// </summary>
        public Guid WikiFK { get; set; }

        /// <summary>
        /// Gets or sets the canonical full namespace path of the page within
        /// its wiki (e.g. <c>engineering/onboarding/setup</c>). The addressing
        /// source of truth; the final segment becomes the page's
        /// <see cref="WikiPageContentReadDto.Slug"/> leaf.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the page title. Applied to the page row on save (a new
        /// page is created with this title; an existing page's title is
        /// updated).
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional page description/summary.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the raw body text being saved (e.g. markdown source).
        /// The server stores these bytes through the body store and pins the
        /// resulting version to the returned content hash.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the declared content-format key of <see cref="Body"/>
        /// (e.g. <c>markdown</c>), per the ADR-018E content-format DSL. When
        /// empty the server applies its default content format.
        /// </summary>
        public string ContentFormatKey { get; set; } = string.Empty;
    }
}
