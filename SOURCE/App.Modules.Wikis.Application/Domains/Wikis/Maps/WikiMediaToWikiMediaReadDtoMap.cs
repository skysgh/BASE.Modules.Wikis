using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Forward map: <see cref="WikiMedia"/> → <see cref="WikiMediaReadDto"/>.
    /// Used for GET operations and IQueryable projections.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    public class WikiMediaToWikiMediaReadDtoMap : ObjectMapBase<WikiMedia, WikiMediaReadDto>
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
