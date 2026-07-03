using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Reverse map: <see cref="WikiAclWriteDto"/> → <see cref="WikiAcl"/>.
    /// Used for POST (create) and PUT (update) operations.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    /// <remarks>
    /// Infrastructure properties and navigation references remain at their
    /// entity defaults — the framework sets them on save.
    /// </remarks>
    public class WikiAclWriteDtoToWikiAclMap : ObjectMapBase<WikiAclWriteDto, WikiAcl>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapFrom(dest => dest.WikiFK, src => src.WikiFK)
                .MapFrom(dest => dest.WikiPageFK, src => src.WikiPageFK)
                .MapFrom(dest => dest.PrincipalId, src => src.PrincipalId)
                .MapFrom(dest => dest.PrincipalType, src => src.PrincipalType)
                .MapFrom(dest => dest.PermissionKey, src => src.PermissionKey);
        }
    }
}
