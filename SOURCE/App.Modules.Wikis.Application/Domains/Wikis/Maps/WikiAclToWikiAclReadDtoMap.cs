using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Forward map: <see cref="WikiAcl"/> → <see cref="WikiAclReadDto"/>.
    /// Used for GET operations and IQueryable projections.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    public class WikiAclToWikiAclReadDtoMap : ObjectMapBase<WikiAcl, WikiAclReadDto>
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
