using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Forward map: <see cref="WikiPageVersion"/> → <see cref="WikiPageVersionReadDto"/>.
    /// Used for GET operations and IQueryable projections.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    public class WikiPageVersionToWikiPageVersionReadDtoMap
        : ObjectMapBase<WikiPageVersion, WikiPageVersionReadDto>
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
