using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Interfaces.API.REST.Constants;
using App.Modules.Sys.Interfaces.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// REST API controller for WikiAcl (share-based access-control) operations.
    /// </summary>
    /// <remarks>
    /// Inherits the standard CRUST endpoints from
    /// <see cref="CrudStateControllerBase{TReadDto,TCreateDto,TUpdateDto}"/>.
    /// ACL rows grant principals access to a wiki or page scope. OData provides
    /// filtering, paging, and sorting; global middleware enforces MaxTop.
    /// Authorization is handled in the base controller via the service's
    /// share-based policy resolution.
    /// </remarks>
    [Route(ApiRoutes.Rest.V1.WikiAcls.Base)]
    public class WikiAclController
        : CrudStateControllerBase<WikiAclReadDto, WikiAclWriteDto, WikiAclWriteDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiAclController"/> class.
        /// </summary>
        /// <param name="service">The WikiAcl application service.</param>
        public WikiAclController(IWikiAclApplicationService service)
            : base(service)
        {
        }
    }
}
