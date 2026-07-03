using App.Modules.Sys.Interfaces.Controllers.Base;
using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Interfaces.API.REST.Constants;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// REST API controller for WikiNodeStyle additive page-style operations.
    /// </summary>
    [Route(ApiRoutes.Rest.V1.WikiNodeStyles.Base)]
    public class WikiNodeStyleController : CrudStateControllerBase<WikiNodeStyleReadDto, WikiNodeStyleWriteDto, WikiNodeStyleWriteDto>
    {
        private readonly IWikiNodeStyleApplicationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiNodeStyleController"/> class.
        /// </summary>
        /// <param name="service">The WikiNodeStyle application service.</param>
        public WikiNodeStyleController(IWikiNodeStyleApplicationService service)
            : base(service)
        {
            this._service = service;
        }

        /// <summary>
        /// Returns the additive node-style rows for a single wiki page.
        /// </summary>
        [HttpGet(ApiRoutes.Rest.V1.WikiNodeStyles.ByPageAction)]
        [ProducesResponseType(typeof(IReadOnlyList<WikiNodeStyleReadDto>), 200)]
        public async Task<ActionResult<IReadOnlyList<WikiNodeStyleReadDto>>> GetForPageAsync(
            Guid wikiPageId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<WikiNodeStyleReadDto> result = await this._service
                .GetForPageAsync(wikiPageId, cancellationToken)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
    }
}
