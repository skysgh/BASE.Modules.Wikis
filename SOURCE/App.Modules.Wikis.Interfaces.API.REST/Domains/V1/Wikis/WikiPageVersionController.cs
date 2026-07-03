using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Interfaces.API.REST.Constants;
using App.Modules.Sys.Interfaces.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// REST API controller for WikiPageVersion operations.
    /// </summary>
    /// <remarks>
    /// Inherits the standard CRUST endpoints from
    /// <see cref="CrudStateControllerBase{TReadDto,TCreateDto,TUpdateDto}"/>.
    /// Versions are immutable snapshots; the create surface appends a new
    /// version. OData provides filtering, paging, and sorting; global middleware
    /// enforces MaxTop. Authorization is handled in the base controller via the
    /// service's share-based policy resolution.
    /// </remarks>
    [Route(ApiRoutes.Rest.V1.WikiPageVersions.Base)]
    public class WikiPageVersionController
        : CrudStateControllerBase<WikiPageVersionReadDto, WikiPageVersionWriteDto, WikiPageVersionWriteDto>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="WikiPageVersionController"/> class.
        /// </summary>
        /// <param name="service">The WikiPageVersion application service.</param>
        public WikiPageVersionController(IWikiPageVersionApplicationService service)
            : base(service)
        {
        }
    }
}
