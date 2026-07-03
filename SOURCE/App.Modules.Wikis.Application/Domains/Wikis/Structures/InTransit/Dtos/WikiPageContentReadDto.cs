using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// A server-composed, render-ready projection of a single
    /// <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPage"/>:
    /// page addressing/metadata, plus the current published
    /// <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPageVersion"/>
    /// metadata, plus that version's inline body text resolved through the body
    /// store.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exists so a reader renders a page from <b>one</b> GET rather than
    /// chaining page → current version → body-bytes calls. It deliberately
    /// inlines the body (the body bytes addressed by the version's locator are
    /// read server-side and returned as <see cref="Body"/>), unlike
    /// <see cref="WikiPageReadDto"/> which is the thin CRUST shape and addresses
    /// versions through their own endpoints.
    /// </para>
    /// <para>
    /// A page that exists but has no published version yet (a fresh page, or a
    /// DokuWiki-style "missing" path) returns with version fields defaulted and
    /// <see cref="HasContent"/> = <c>false</c>, so the client can render the
    /// "this page does not exist yet — create it" invitation without a separate
    /// existence probe.
    /// </para>
    /// </remarks>
    public class WikiPageContentReadDto : IHasGuidId, IHasTitleAndDescription
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the FK to the owning wiki root.
        /// </summary>
        public Guid WikiFK { get; set; }

        /// <summary>
        /// Gets or sets the canonical full namespace path of this page within
        /// its wiki (e.g. <c>engineering/onboarding/setup</c>). The addressing
        /// source of truth; the prefix is the namespace and the final segment is
        /// the leaf.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the derived leaf segment of <see cref="Path"/>.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the FK to the currently published version, or
        /// <c>null</c> when the page has no published version yet.
        /// </summary>
        public Guid? CurrentVersionId { get; set; }

        /// <summary>
        /// Gets or sets the 1-based version number of the current published
        /// version, or <c>0</c> when there is none.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the declared content-format key of <see cref="Body"/>
        /// (e.g. <c>markdown</c>), per the ADR-018E content-format DSL. Empty
        /// when there is no current version.
        /// </summary>
        public string ContentFormatKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sink-independent content hash of the current
        /// version's body, for client-side drift detection. Empty when there is
        /// no current version.
        /// </summary>
        public string ContentHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the inline body text of the current published version,
        /// resolved server-side through the body store. Empty when the page has
        /// no published version (a fresh or DokuWiki-style "missing" page).
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this page has a resolvable
        /// published version with body content. <c>false</c> signals the client
        /// to render the "create this page" invitation.
        /// </summary>
        public bool HasContent { get; set; }
    }
}
