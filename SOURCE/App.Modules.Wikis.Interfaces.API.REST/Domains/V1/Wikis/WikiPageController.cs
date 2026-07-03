using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Interfaces.API.REST.Constants;
using App.Modules.Sys.Interfaces.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// REST API controller for WikiPage operations.
    /// </summary>
    /// <remarks>
    /// Inherits the standard CRUST endpoints from
    /// <see cref="CrudStateControllerBase{TReadDto,TCreateDto,TUpdateDto}"/>.
    /// OData provides filtering, paging, and sorting; global middleware enforces
    /// MaxTop. Authorization is handled in the base controller via the service's
    /// share-based policy resolution, not simplistic role attributes.
    /// <para>
    /// In addition to the inherited CRUST surface (which returns the thin page
    /// row), this controller exposes a server-composed single-GET render
    /// projection (<c>{id}/content</c> and a path-addressed variant) so a reader
    /// renders a page from one call rather than chaining page → current version →
    /// body-bytes requests.
    /// </para>
    /// </remarks>
    [Route(ApiRoutes.Rest.V1.WikiPages.Base)]
    public class WikiPageController
        : CrudStateControllerBase<WikiPageReadDto, WikiPageWriteDto, WikiPageWriteDto>
    {
        private readonly IWikiPageApplicationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageController"/> class.
        /// </summary>
        /// <param name="service">The WikiPage application service.</param>
        public WikiPageController(IWikiPageApplicationService service)
            : base(service)
        {
            this._service = service;
        }

        /// <summary>
        /// Lists the thin page rows that belong to one wiki root, ordered by
        /// canonical path for stable client-side tree construction.
        /// </summary>
        /// <param name="wikiId">The owning wiki root id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The ordered page rows for the wiki root.</returns>
        /// <response code="200">The ordered page rows.</response>
        [HttpGet(ApiRoutes.Rest.V1.WikiPages.ByWikiAction)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IReadOnlyList<WikiPageReadDto>>> GetByWikiAsync(
            Guid wikiId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<WikiPageReadDto> pages = await this._service
                .GetPagesByWikiAsync(wikiId, cancellationToken)
                .ConfigureAwait(false);

            return this.Ok(pages);
        }

        /// <summary>
        /// Gets the server-composed render projection for a page by its id:
        /// page addressing/metadata plus the current published version's
        /// metadata and inline body text.
        /// </summary>
        /// <param name="id">The page id from the route.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The composed <see cref="WikiPageContentReadDto"/>. A page that exists
        /// but has no published version returns with
        /// <see cref="WikiPageContentReadDto.HasContent"/> = <c>false</c>.
        /// </returns>
        /// <response code="200">The composed page content.</response>
        /// <response code="404">No page exists with that id.</response>
        [HttpGet(ApiRoutes.Rest.V1.WikiPages.ContentByIdAction)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WikiPageContentReadDto>> GetContentByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            WikiPageContentReadDto? content = await this._service
                .GetContentByIdAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (content is null)
            {
                return this.NotFound();
            }

            return content;
        }

        /// <summary>
        /// Gets the server-composed render projection for a page by its
        /// canonical DokuWiki-style path within a wiki root.
        /// </summary>
        /// <param name="wikiId">The owning wiki root id from the route.</param>
        /// <param name="path">
        /// The full slash-shaped page path from the catch-all route segment
        /// (e.g. <c>engineering/onboarding/setup</c>).
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The composed <see cref="WikiPageContentReadDto"/>. A path whose page
        /// does not exist yet returns with
        /// <see cref="WikiPageContentReadDto.HasContent"/> = <c>false</c> so the
        /// client renders the "create this page" invitation.
        /// </returns>
        /// <response code="200">The composed page content (may be a create invitation).</response>
        /// <response code="404">No page exists at that path.</response>
        [HttpGet(ApiRoutes.Rest.V1.WikiPages.ContentByPathAction)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WikiPageContentReadDto>> GetContentByPathAsync(
            Guid wikiId,
            string path,
            CancellationToken cancellationToken = default)
        {
            WikiPageContentReadDto? content = await this._service
                .GetContentByPathAsync(wikiId, path, cancellationToken)
                .ConfigureAwait(false);

            if (content is null)
            {
                return this.NotFound();
            }

            return content;
        }

        /// <summary>
        /// Saves an edit to a page's content: stores the submitted body, appends
        /// a new immutable version, and repoints the page's current pointer. When
        /// no page exists at the submitted wiki/path it is created first
        /// (DokuWiki-style "create this page").
        /// </summary>
        /// <param name="request">The content to save (addressing + body).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The composed <see cref="WikiPageContentReadDto"/> for the saved page,
        /// reflecting the newly published version and its inline body.
        /// </returns>
        /// <response code="200">The composed page content after the save.</response>
        [HttpPost(ApiRoutes.Rest.V1.WikiPages.SaveContentAction)]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<WikiPageContentReadDto>> SaveContentAsync(
            [FromBody] WikiPageContentWriteDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                WikiPageContentReadDto content = await this._service
                    .SaveContentAsync(request, cancellationToken)
                    .ConfigureAwait(false);

                return content;
            }
            catch (UnauthorizedAccessException exception)
            {
                return this.StatusCode(StatusCodes.Status403Forbidden, exception.Message);
            }
        }
    }
}
