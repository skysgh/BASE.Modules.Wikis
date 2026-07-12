using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Interfaces.API.REST.Constants;
using App.Modules.Sys.Interfaces.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// REST API controller for Wiki root operations.
    /// </summary>
    /// <remarks>
    /// <b>No authorization attributes on this controller — this is intentional (ADR-027).</b>
    /// Authorization is a property of the data, not of the endpoint. Reads through this
    /// controller are filtered by the ADR-020 central pre-query handler against ADR-013
    /// <c>Share</c> rows; writes are gated by the pre-commit handler in the same pipeline.
    /// If the caller cannot see or write a row, the persistence layer refuses it — this
    /// controller does not need to know. See ADR-027 for the full rationale, including
    /// why <c>[Authorize]</c>, <c>[AllowAnonymous]</c>, <c>[DemandPermission]</c>, and
    /// <c>[RequirePermission]</c> are forbidden here.
    /// </remarks>
    /// <remarks>
    /// Inherits the standard CRUST endpoints from
    /// <see cref="CrudStateControllerBase{TReadDto,TCreateDto,TUpdateDto}"/>.
    /// OData provides filtering, paging, and sorting; global middleware enforces
    /// MaxTop. Authorization is handled in the base controller via the service's
    /// share-based policy resolution, not simplistic role attributes.
    /// </remarks>
    [Route(ApiRoutes.Rest.V1.Wikis.Base)]
    public class WikiController
        : CrudStateControllerBase<WikiReadDto, WikiWriteDto, WikiWriteDto>
    {
        private readonly IWikiApplicationService _wikiApplicationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiController"/> class.
        /// </summary>
        /// <param name="service">The Wiki application service.</param>
        public WikiController(IWikiApplicationService service)
            : base(service)
        {
            this._wikiApplicationService = service;
        }

        /// <summary>
        /// Returns the operator-configured client-side defaults for the wiki
        /// host component: default store key, default slug, root document name,
        /// and no-content message. No authentication is required because these
        /// are display-only defaults needed before any session is established.
        /// </summary>
        /// <returns>A <see cref="WikiClientConfigReadDto"/> populated from <c>appsettings.json</c>.</returns>
        [HttpGet(ApiRoutes.Rest.V1.Wikis.ClientConfigAction)]
        public ActionResult<WikiClientConfigReadDto> GetClientConfig()
        {
            return this.Ok(this._wikiApplicationService.GetClientConfig());
        }
    }
}
