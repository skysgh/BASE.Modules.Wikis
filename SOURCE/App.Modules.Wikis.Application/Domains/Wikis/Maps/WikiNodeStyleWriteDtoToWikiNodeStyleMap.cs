using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Structures.AtRest.Enums;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Reverse map: <see cref="WikiNodeStyleWriteDto"/> → <see cref="WikiNodeStyle"/>.
    /// </summary>
    public class WikiNodeStyleWriteDtoToWikiNodeStyleMap : ObjectMapBase<WikiNodeStyleWriteDto, WikiNodeStyle>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapFrom(dest => dest.WikiPageFK, src => src.WikiPageFK)
                .MapFrom(dest => dest.SectionKey, src => src.SectionKey)
                .MapFrom(dest => dest.BackgroundMediaName, src => src.BackgroundMediaName)
                .MapFrom(dest => dest.OverlayOpacityMode, src => (WikiNodeStyleOverlayOpacityMode)src.OverlayOpacityMode)
                .MapFrom(dest => dest.ContrastMode, src => (WikiNodeStyleContrastMode)src.ContrastMode);
        }
    }
}
