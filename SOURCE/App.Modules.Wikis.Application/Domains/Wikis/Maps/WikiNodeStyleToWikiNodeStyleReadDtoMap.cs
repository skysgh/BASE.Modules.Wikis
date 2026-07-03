using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Forward map: <see cref="WikiNodeStyle"/> → <see cref="WikiNodeStyleReadDto"/>.
    /// </summary>
    public class WikiNodeStyleToWikiNodeStyleReadDtoMap : ObjectMapBase<WikiNodeStyle, WikiNodeStyleReadDto>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapFrom(dest => dest.WikiPageFK, src => src.WikiPageFK)
                .MapFrom(dest => dest.SectionKey, src => src.SectionKey)
                .MapFrom(dest => dest.BackgroundMediaName, src => src.BackgroundMediaName)
                .MapFrom(dest => dest.OverlayOpacityMode, src => (int)src.OverlayOpacityMode)
                .MapFrom(dest => dest.ContrastMode, src => (int)src.ContrastMode);
        }
    }
}
