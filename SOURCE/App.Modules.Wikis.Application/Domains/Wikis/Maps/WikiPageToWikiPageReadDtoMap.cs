using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Forward map: <see cref="WikiPage"/> → <see cref="WikiPageReadDto"/>.
    /// Used for GET operations and IQueryable projections.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    public class WikiPageToWikiPageReadDtoMap : ObjectMapBase<WikiPage, WikiPageReadDto>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapTitleAndDescription()
                .MapEnabled()
                .MapFrom(dest => dest.WikiFK, src => src.WikiFK)
                .MapFrom(dest => dest.ParentWikiPageFK, src => src.ParentWikiPageFK)
                .MapFrom(dest => dest.Path, src => src.Path)
                .MapFrom(dest => dest.Slug, src => src.Slug)
                .MapFrom(dest => dest.CurrentVersionId, src => src.CurrentVersionId);
        }
    }
}
