using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Maps
{
    /// <summary>
    /// Reverse map: <see cref="WikiWriteDto"/> → <see cref="Wiki"/>.
    /// Used for POST (create) and PUT (update) operations.
    /// Discovered at startup via the object-map reflection scan.
    /// </summary>
    /// <remarks>
    /// Reverse maps only set the properties the DTO carries. Infrastructure
    /// properties (timestamps, record state, audit fields) and navigation
    /// collections remain at their entity defaults — the framework sets them on
    /// save.
    /// </remarks>
    public class WikiWriteDtoToWikiMap : ObjectMapBase<WikiWriteDto, Wiki>
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
