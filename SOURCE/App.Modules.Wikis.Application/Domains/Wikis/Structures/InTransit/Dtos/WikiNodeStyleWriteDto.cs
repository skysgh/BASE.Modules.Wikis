using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Write DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiNodeStyle"/>.
    /// Used for POST and PUT operations.
    /// </summary>
    public class WikiNodeStyleWriteDto : IHasGuidId
    {
        /// <inheritdoc />
        public Guid Id { get; set; }

        /// <summary>
        /// Owning wiki page id whose rendered output is decorated.
        /// </summary>
        public Guid WikiPageFK { get; set; }

        /// <summary>
        /// Stable markdown heading id / section key to decorate. Empty means page-level.
        /// </summary>
        public string SectionKey { get; set; } = string.Empty;

        /// <summary>
        /// Logical page-vicinity media reference used as the background asset.
        /// </summary>
        public string BackgroundMediaName { get; set; } = string.Empty;

        /// <summary>
        /// Bounded overlay-opacity mode.
        /// </summary>
        public int OverlayOpacityMode { get; set; }

        /// <summary>
        /// Bounded text-contrast mode.
        /// </summary>
        public int ContrastMode { get; set; }
    }
}
