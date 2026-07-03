using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Reverse map: <see cref="WikiPageWriteDto"/> → <see cref="WikiPage"/>.
    /// Used for POST (create) and PUT (update) operations.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    /// <remarks>
    /// Reverse maps only set the properties the DTO carries. Navigation
    /// collections and infrastructure properties remain at their entity
    /// defaults — the framework sets them on save.
    /// </remarks>
    public class WikiPageWriteDtoToWikiPageMap : ObjectMapBase<WikiPageWriteDto, WikiPage>
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
                .MapFrom(dest => dest.Slug, src => src.Slug)
                .MapFrom(dest => dest.CurrentVersionId, src => src.CurrentVersionId);
        }
    }
}
