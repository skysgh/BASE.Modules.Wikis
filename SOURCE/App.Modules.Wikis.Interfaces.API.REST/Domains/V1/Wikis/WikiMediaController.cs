using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Interfaces.API.REST.Constants;
using App.Modules.Sys.Interfaces.Controllers.Base;
using App.Modules.Sys.Shared.ObjectStorage.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// REST API controller for WikiMedia operations.
    /// </summary>
    /// <remarks>
    /// Inherits the standard CRUST endpoints from
    /// <see cref="CrudStateControllerBase{TReadDto,TCreateDto,TUpdateDto}"/>.
    /// Media handles are immutable; the create surface registers a new blob
    /// handle. OData provides filtering, paging, and sorting; global middleware
    /// enforces MaxTop. Authorization is handled in the base controller via the
    /// service's share-based policy resolution.
    /// <para>
    /// In addition to the inherited CRUST surface (which addresses the immutable
    /// media <i>handle</i>), this controller exposes a byte-level round trip
    /// (<c>POST/GET {id}/bytes</c>) that addresses the underlying object-store
    /// <i>bytes</i>. The upload allocates a fresh immutable blob id; the download
    /// streams the stored bytes back with their authoritative media type.
    /// </para>
    /// </remarks>
    [Route(ApiRoutes.Rest.V1.WikiMedias.Base)]
    public class WikiMediaController
        : CrudStateControllerBase<WikiMediaReadDto, WikiMediaWriteDto, WikiMediaWriteDto>
    {
        private readonly IWikiMediaApplicationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiMediaController"/> class.
        /// </summary>
        /// <param name="service">The WikiMedia application service.</param>
        public WikiMediaController(IWikiMediaApplicationService service)
            : base(service)
        {
            this._service = service;
        }

        /// <summary>
        /// Uploads the actual media <b>bytes</b> for a page and registers the
        /// immutable media handle that addresses them (A4 write half, ADR-018).
        /// </summary>
        /// <remarks>
        /// The handle's identity and integrity are owned by the store, not the
        /// caller: a fresh immutable blob id is allocated and the content hash is
        /// computed during storage. The caller supplies only the owning page,
        /// the optional title/description, and the file itself.
        /// </remarks>
        /// <param name="id">
        /// The media handle id from the route. Present for symmetry with the
        /// download action and future idempotent re-points; the create path
        /// allocates its own identity, so the value is not used to mutate an
        /// existing handle.
        /// </param>
        /// <param name="wikiPageId">The owning page the media is attached to.</param>
        /// <param name="file">The uploaded media file (multipart/form-data).</param>
        /// <param name="title">Optional human-readable title for the media.</param>
        /// <param name="description">Optional description for the media.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created media handle as a read DTO.</returns>
        /// <response code="201">Media stored and handle created.</response>
        /// <response code="400">No file supplied or validation failure.</response>
        /// <response code="403">
        /// The current principal is not permitted to manage media on the page.
        /// </response>
        [HttpPost(ApiRoutes.Rest.V1.WikiMedias.BytesAction)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<WikiMediaReadDto>> UploadBytesAsync(
            Guid id,
            [FromForm] Guid wikiPageId,
            IFormFile file,
            [FromForm] string? title,
            [FromForm] string? description,
            CancellationToken cancellationToken = default)
        {
            if (file is null || file.Length == 0)
            {
                return this.BadRequest("A non-empty media file is required.");
            }

            WikiMediaWriteDto metadata = new WikiMediaWriteDto
            {
                WikiPageFK = wikiPageId,
                MediaType = file.ContentType,
                Title = title ?? string.Empty,
                Description = description ?? string.Empty,
            };

            await using Stream content = file.OpenReadStream();

            try
            {
                WikiMediaReadDto result = await this._service
                    .StoreMediaAsync(metadata, content, cancellationToken)
                    .ConfigureAwait(false);

                return this.CreatedAtAction(nameof(this.GetById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException exception)
            {
                // Share-based denial from the Application layer surfaces as 403.
                // An explicit status code is used (not Forbid()) because this
                // domain has no ASP.NET authentication scheme to challenge.
                return this.StatusCode(StatusCodes.Status403Forbidden, exception.Message);
            }
        }

        /// <summary>
        /// Streams the actual media <b>bytes</b> for a stored media handle
        /// (A4 read half, ADR-018).
        /// </summary>
        /// <remarks>
        /// Honours the deployment's configured
        /// <see cref="App.Modules.Sys.Shared.ObjectStorage.Models.Enums.MediaDeliveryMode"/>:
        /// in <c>Proxy</c> mode the bytes are streamed through this endpoint; in
        /// <c>Direct</c> mode the caller is redirected to a short-lived signed
        /// storage URL and fetches the bytes straight from storage. Both paths
        /// are produced only after the Application-layer share-based access check
        /// passes; a denied or non-existent handle is reported identically as
        /// <c>404</c> so the endpoint never reveals content the reader cannot see.
        /// </remarks>
        /// <param name="id">The id of the media handle to retrieve bytes for.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The media byte stream, or a redirect to a signed URL.</returns>
        /// <response code="200">Media bytes streamed (Proxy mode).</response>
        /// <response code="302">Redirect to a signed storage URL (Direct mode).</response>
        /// <response code="404">No accessible media handle with the given id exists.</response>
        [HttpGet(ApiRoutes.Rest.V1.WikiMedias.BytesAction)]
        [ProducesResponseType(200)]
        [ProducesResponseType(302)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBytesAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            WikiMediaRetrievalResult? result = await this._service
                .GetMediaForDeliveryAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (result is null)
            {
                return this.NotFound();
            }

            if (result.Mode == MediaDeliveryMode.Direct && result.SignedUrl is not null)
            {
                // Direct delivery: hand the client a short-lived signed URL so it
                // fetches the bytes straight from storage (requires storage CORS).
                return this.Redirect(result.SignedUrl);
            }

            // Proxy delivery (default): stream the bytes through the backend.
            return this.File(result.Content!, result.MediaType);
        }

        /// <summary>
        /// Stores a draw.io diagram as its two-artifact <b>pair</b> — an editable
        /// source (mxfile) plus a display render (SVG) — and links the render
        /// back to the source for reopen (ADR-018).
        /// </summary>
        /// <remarks>
        /// Both artifacts are stored under the same owning page through the same
        /// A4 byte pipeline; a single share-based authorization check governs
        /// both writes, so a partial pair can never be created by an unauthorized
        /// caller. Authored content references the returned <i>render</i> via a
        /// <c>drawio:{id}</c> token; "edit diagram" later resolves the source
        /// from the render via <see cref="GetDiagramSourceAsync"/>. The media
        /// types are fixed to the diagram pair, so no content type is taken from
        /// the caller.
        /// </remarks>
        /// <param name="wikiPageId">The owning page the diagram is attached to.</param>
        /// <param name="source">The diagram source (mxfile) file (multipart/form-data).</param>
        /// <param name="render">The diagram render (SVG) file (multipart/form-data).</param>
        /// <param name="title">Optional human-readable title for the diagram.</param>
        /// <param name="description">Optional description for the diagram.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created render media handle as a read DTO.</returns>
        /// <response code="201">Diagram pair stored; the render handle is returned.</response>
        /// <response code="400">A required file was not supplied.</response>
        /// <response code="403">
        /// The current principal is not permitted to manage media on the page.
        /// </response>
        [HttpPost(ApiRoutes.Rest.V1.WikiMedias.DiagramAction)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<WikiMediaReadDto>> UploadDiagramAsync(
            [FromForm] Guid wikiPageId,
            IFormFile source,
            IFormFile render,
            [FromForm] string? title,
            [FromForm] string? description,
            CancellationToken cancellationToken = default)
        {
            if (source is null || source.Length == 0)
            {
                return this.BadRequest("A non-empty diagram source file is required.");
            }

            if (render is null || render.Length == 0)
            {
                return this.BadRequest("A non-empty diagram render file is required.");
            }

            WikiMediaWriteDto metadata = new WikiMediaWriteDto
            {
                WikiPageFK = wikiPageId,
                Title = title ?? string.Empty,
                Description = description ?? string.Empty,
            };

            await using Stream sourceContent = source.OpenReadStream();
            await using Stream renderContent = render.OpenReadStream();

            try
            {
                (WikiMediaReadDto Render, WikiMediaReadDto Source) result = await this._service
                    .StoreDiagramAsync(metadata, sourceContent, renderContent, cancellationToken)
                    .ConfigureAwait(false);

                // The render is the addressable artifact authored content points
                // at; return it as the created resource.
                return this.CreatedAtAction(
                    nameof(this.GetById),
                    new { id = result.Render.Id },
                    result.Render);
            }
            catch (UnauthorizedAccessException exception)
            {
                // Share-based denial from the Application layer surfaces as 403.
                return this.StatusCode(StatusCodes.Status403Forbidden, exception.Message);
            }
        }

        /// <summary>
        /// Resolves the editable <b>source</b> (mxfile) handle for a stored
        /// diagram <b>render</b> so the editor can reopen it for edit (ADR-018).
        /// </summary>
        /// <remarks>
        /// The render is identified by the same media id used by its
        /// <c>drawio:{id}</c> token. The returned source handle's bytes are then
        /// fetched through the existing <c>GET {id}/bytes</c> action. A denied,
        /// missing, or unlinked render is reported identically as <c>404</c> so
        /// the endpoint never reveals content the reader cannot see.
        /// </remarks>
        /// <param name="id">The id of the render media handle.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The linked source media handle as a read DTO.</returns>
        /// <response code="200">The linked source handle is returned.</response>
        /// <response code="404">
        /// No accessible render with the given id exists, or it has no linked source.
        /// </response>
        [HttpGet(ApiRoutes.Rest.V1.WikiMedias.DiagramSourceAction)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WikiMediaReadDto>> GetDiagramSourceAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            WikiMediaReadDto? source = await this._service
                .GetDiagramSourceAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (source is null)
            {
                return this.NotFound();
            }

            return source;
        }
    }
}
