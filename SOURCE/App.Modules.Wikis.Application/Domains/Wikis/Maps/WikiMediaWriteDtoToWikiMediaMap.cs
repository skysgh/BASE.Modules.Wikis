using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Reverse map: <see cref="WikiMediaWriteDto"/> → <see cref="WikiMedia"/>.
    /// Used for POST (create) operations.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    /// <remarks>
    /// Media handles are immutable; "replace" means a new blob and a fresh row.
    /// Infrastructure properties remain at their entity defaults — the framework
    /// sets them on save.
    /// </remarks>
    public class WikiMediaWriteDtoToWikiMediaMap : ObjectMapBase<WikiMediaWriteDto, WikiMedia>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapTitleAndDescription()
                .MapFrom(dest => dest.WikiPageFK, src => src.WikiPageFK)
                .MapFrom(dest => dest.BlobId, src => src.BlobId)
                .MapFrom(dest => dest.MediaType, src => src.MediaType)
                .MapFrom(dest => dest.ContentHash, src => src.ContentHash)
                .MapFrom(dest => dest.SourceMediaFK, src => src.SourceMediaFK);
        }
    }
}
