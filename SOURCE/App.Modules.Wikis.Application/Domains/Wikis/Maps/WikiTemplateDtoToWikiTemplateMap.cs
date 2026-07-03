using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Reverse map: <see cref="WikiTemplateDto"/> → <see cref="WikiTemplate"/>.
    /// Used for CREATE and UPDATE operations (ADR-018C).
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    public class WikiTemplateDtoToWikiTemplateMap : ObjectMapBase<WikiTemplateDto, WikiTemplate>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapFrom(dest => dest.WikiFK, src => src.WikiFK)
                .MapKey()
                .MapTitleAndDescription()
                .MapEnabled()
                .MapFrom(dest => dest.ContentFormatKey, src => src.ContentFormatKey);
        }
    }
}
