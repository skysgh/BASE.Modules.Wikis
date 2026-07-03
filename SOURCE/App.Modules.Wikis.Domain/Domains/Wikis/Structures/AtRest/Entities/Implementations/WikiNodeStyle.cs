using App.Modules.Sys.Shared.Models.Base;
using App.Modules.Wikis.Domain.Domains.Wikis.Structures.AtRest.Enums;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// Additive page-scoped style row describing a bounded section or page-level
    /// background presentation (ADR-018D §2.2, build-plan Phase D step 16).
    /// </summary>
    /// <remarks>
    /// A <see cref="WikiNodeStyle"/> row is additive page data, not page markup:
    /// it hangs off a <see cref="WikiPage"/> and optionally targets a single
    /// rendered section via <see cref="SectionKey"/> (matching the markdown
    /// heading id / slug). When <see cref="SectionKey"/> is null or empty, the
    /// style applies at page level.
    /// <para>
    /// The knob set is intentionally bounded: background media is referenced by
    /// logical page-vicinity media name, and presentation is constrained to
    /// overlay opacity plus text contrast mode. No free CSS, HTML, or arbitrary
    /// style fragments are persisted here.
    /// </para>
    /// </remarks>
    public class WikiNodeStyle : DefaultEntityBase
    {
        /// <summary>
        /// FK to the owning <see cref="WikiPage"/> whose rendered output this
        /// style decorates.
        /// </summary>
        public Guid WikiPageFK { get; set; }

        /// <summary>
        /// Stable section key (markdown heading slug / rendered heading id) this
        /// style applies to. Null or empty means the style decorates the whole
        /// page wrapper rather than one specific section.
        /// </summary>
        public string SectionKey { get; set; } = string.Empty;

        /// <summary>
        /// Logical page-vicinity media name (for example, <c>media:hero.png</c>)
        /// used as the bounded background asset reference.
        /// </summary>
        public string BackgroundMediaName { get; set; } = string.Empty;

        /// <summary>
        /// Bounded overlay opacity mode controlling how strongly the background
        /// image is muted behind foreground content.
        /// </summary>
        public WikiNodeStyleOverlayOpacityMode OverlayOpacityMode { get; set; }

        /// <summary>
        /// Bounded text contrast mode controlling the foreground treatment over
        /// the background image.
        /// </summary>
        public WikiNodeStyleContrastMode ContrastMode { get; set; }

        /// <summary>
        /// Navigation: the owning page whose rendered output is decorated.
        /// </summary>
        public WikiPage? Page { get; set; }
    }
}
