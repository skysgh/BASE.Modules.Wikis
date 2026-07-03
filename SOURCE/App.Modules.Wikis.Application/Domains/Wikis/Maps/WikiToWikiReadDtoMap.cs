using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Forward map: <see cref="Wiki"/> → <see cref="WikiReadDto"/>.
    /// Used for GET operations and IQueryable projections.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    public class WikiToWikiReadDtoMap : ObjectMapBase<Wiki, WikiReadDto>
    {
        /// <inheritdoc />
        protected override void ConfigureMapping()
        {
            this.CreateMap()
                .MapGuidId()
                .MapKey()
                .MapTitleAndDescription()
                .MapEnabled()
                .MapFrom(dest => dest.OwnerWorkspaceId, src => src.OwnerWorkspaceId);
        }
    }
}
