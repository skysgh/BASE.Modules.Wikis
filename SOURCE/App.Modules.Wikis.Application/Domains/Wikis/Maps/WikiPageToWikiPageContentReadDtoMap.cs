using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Forward map: <see cref="WikiPage"/> → <see cref="WikiPageContentReadDto"/>.
    /// Maps only the page-derived addressing/metadata. The current-version
    /// metadata (<see cref="WikiPageContentReadDto.VersionNumber"/>,
    /// <see cref="WikiPageContentReadDto.ContentFormatKey"/>,
    /// <see cref="WikiPageContentReadDto.ContentHash"/>), the inline
    /// <see cref="WikiPageContentReadDto.Body"/>, and
    /// <see cref="WikiPageContentReadDto.HasContent"/> are composed by
    /// <see cref="Services.IWikiPageApplicationService"/> after the body store
    /// resolves the bytes, so they are explicitly ignored here per the
    /// every-property-accounted-for mapping rule.
    /// </summary>
    /// <remarks>Discovered at startup via the object-map reflection scan.</remarks>
    public class WikiPageToWikiPageContentReadDtoMap : ObjectMapBase<WikiPage, WikiPageContentReadDto>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapTitleAndDescription()
                .MapFrom(dest => dest.WikiFK, src => src.WikiFK)
                .MapFrom(dest => dest.Path, src => src.Path)
                .MapFrom(dest => dest.Slug, src => src.Slug)
                .MapFrom(dest => dest.CurrentVersionId, src => src.CurrentVersionId)
                .Ignore(dest => dest.VersionNumber)
                .Ignore(dest => dest.ContentFormatKey)
                .Ignore(dest => dest.ContentHash)
                .Ignore(dest => dest.Body)
                .Ignore(dest => dest.HasContent);
        }
    }
}
