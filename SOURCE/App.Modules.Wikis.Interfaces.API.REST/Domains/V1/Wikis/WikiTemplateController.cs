using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Interfaces.API.REST.Constants;
using App.Modules.Sys.Interfaces.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// REST API controller for WikiTemplate operations (ADR-018C templates-as-pages).
    /// </summary>
    /// <remarks>
    /// Inherits the standard CRUST endpoints from
    /// <see cref="SimpleCrudStateControllerBase{TDto}"/>.
    /// OData provides filtering, paging, and sorting; global middleware enforces
    /// MaxTop. Authorization is handled in the base controller via the service's
    /// share-based policy resolution, not simplistic role attributes.
    /// </remarks>
    [Route(ApiRoutes.Rest.V1.WikiTemplates.Base)]
    public class WikiTemplateController
        : SimpleCrudStateControllerBase<WikiTemplateDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiTemplateController"/> class.
        /// </summary>
        /// <param name="service">The WikiTemplate application service.</param>
        public WikiTemplateController(IWikiTemplateAppService service)
            : base(service)
        {
        }
    }
}
