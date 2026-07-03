using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Reverse map: <see cref="WikiPageVersionWriteDto"/> → <see cref="WikiPageVersion"/>.
    /// Used for POST (create) operations.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    /// <remarks>
    /// Versions are immutable snapshots; the write surface is used to append a
    /// new version. Infrastructure properties remain at their entity defaults —
    /// the framework sets them on save.
    /// </remarks>
    public class WikiPageVersionWriteDtoToWikiPageVersionMap
        : ObjectMapBase<WikiPageVersionWriteDto, WikiPageVersion>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapFrom(dest => dest.WikiPageFK, src => src.WikiPageFK)
                .MapFrom(dest => dest.VersionNumber, src => src.VersionNumber)
                .MapFrom(dest => dest.BodyBlobId, src => src.BodyBlobId)
                .MapFrom(dest => dest.ContentHash, src => src.ContentHash)
                .MapFrom(dest => dest.ContentFormatKey, src => src.ContentFormatKey);
        }
    }
}
