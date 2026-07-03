using App.Modules.Sys.Shared.Application;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Application service contract for <see cref="WikiMediaReadDto"/> operations.
    /// Extends <see cref="ICrudStateAppService{TReadDto,TCreateDto,TUpdateDto}"/>
    /// for standard CRUST operations, returning IQueryable for OData filtering,
    /// paging, and sorting at the API boundary.
    /// </summary>
    public interface IWikiMediaApplicationService
        : ICrudStateAppService<WikiMediaReadDto, WikiMediaWriteDto, WikiMediaWriteDto>
    {
        /// <summary>
        /// Stores the actual media <b>bytes</b> in the object store and creates
        /// the immutable <see cref="WikiMediaReadDto"/> handle that addresses
        /// them (A4 byte-level binding, ADR-018).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the write half of the media round trip. A fresh immutable
        /// <c>BlobId</c> is allocated, the bytes are pushed through the Sys media
        /// pipeline (scan → store), and the returned content hash and size are
        /// captured onto the handle. The caller supplies only the descriptive
        /// metadata (owning page, media type, title/description); the
        /// <c>BlobId</c> and <c>ContentHash</c> on the incoming DTO are ignored
        /// and replaced by the storage result, because identity and integrity
        /// are owned by the store, not the caller.
        /// </para>
        /// </remarks>
        /// <param name="metadata">
        /// Descriptive metadata for the media handle (owning page FK, media
        /// type, title, description). Any <c>BlobId</c>/<c>ContentHash</c> on
        /// this DTO is ignored.
        /// </param>
        /// <param name="content">
        /// The media byte stream, positioned at the beginning.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created, persisted media handle.</returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the current principal is granted neither media-management
        /// nor content-authoring access on the owning page (share-based,
        /// Application-layer gate).
        /// </exception>
        Task<WikiMediaReadDto> StoreMediaAsync(
            WikiMediaWriteDto metadata,
            Stream content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the actual media <b>bytes</b> for a stored media handle
        /// (A4 byte-level binding, ADR-018).
        /// </summary>
        /// <remarks>
        /// This is the read half of the media round trip. The handle is loaded,
        /// its object-store path is recomputed deterministically from the
        /// immutable <c>BlobId</c>, and the bytes are streamed back from the
        /// store. Returns <c>null</c> when no handle with the given id exists
        /// <b>or</b> when the current principal is not granted read access — the
        /// two cases are deliberately indistinguishable so the endpoint never
        /// acts as an existence oracle for content the reader cannot see.
        /// </remarks>
        /// <param name="mediaId">The id of the <c>WikiMedia</c> handle.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A tuple of the content stream (the caller must dispose it) and the
        /// authoritative media (MIME) type, or <c>null</c> if the handle does
        /// not exist or access is denied.
        /// </returns>
        Task<(Stream Content, string MediaType)?> GetMediaBytesAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a stored media handle's bytes using the deployment's
        /// configured <see cref="App.Modules.Sys.Shared.ObjectStorage.Models.Enums.MediaDeliveryMode"/>:
        /// either a streamed proxy result (the backend moves the bytes) or a
        /// short-lived signed URL (the client fetches straight from storage).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Both delivery paths are produced only after the same Application-layer
        /// share-based authorization check passes, so they are equally secure;
        /// they differ only in who transfers the bytes. The default mode is the
        /// CORS-free proxy, so a deployment is safe out of the box and opts into
        /// the direct (signed-URL) path explicitly.
        /// </para>
        /// </remarks>
        /// <param name="mediaId">The id of the <c>WikiMedia</c> handle.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A <see cref="WikiMediaRetrievalResult"/> describing the delivery, or
        /// <c>null</c> when the handle does not exist or access is denied.
        /// </returns>
        Task<WikiMediaRetrievalResult?> GetMediaForDeliveryAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores a draw.io diagram as its two-artifact pair — an editable
        /// <b>source</b> (mxfile) plus a display-ready <b>render</b> (SVG) — and
        /// links the render back to the source so the editor can reopen the
        /// source for edit (ADR-018, §10 of the body-storage implementation
        /// note).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Both artifacts are stored through the same A4 byte pipeline as
        /// <see cref="StoreMediaAsync"/> (a fresh immutable <c>BlobId</c> each,
        /// scan → store), under the same owning page. The source is stored first;
        /// the render is then stored with its
        /// <see cref="WikiMediaWriteDto.SourceMediaFK"/> pointing at the source's
        /// id. Authored content references the <em>render</em> via a
        /// <c>drawio:{id}</c> token; "edit diagram" resolves the source from the
        /// render through <see cref="GetDiagramSourceAsync"/>.
        /// </para>
        /// <para>
        /// The single share-based authorization gate (media-management or
        /// content-authoring on the owning page) is performed once and governs
        /// both writes, so a partial pair can never be created by an
        /// unauthorized caller. The media types are fixed to
        /// <see cref="Domain.Domains.Wikis.Constants.WikiDomainConstants.DrawioSourceMediaType"/>
        /// and
        /// <see cref="Domain.Domains.Wikis.Constants.WikiDomainConstants.DrawioRenderMediaType"/>;
        /// any media type on the incoming metadata is ignored.
        /// </para>
        /// </remarks>
        /// <param name="metadata">
        /// Descriptive metadata shared by both artifacts (owning page FK, title,
        /// description). Any <c>BlobId</c>, <c>ContentHash</c>, <c>MediaType</c>,
        /// or <c>SourceMediaFK</c> on this DTO is ignored.
        /// </param>
        /// <param name="source">
        /// The diagram source (mxfile) byte stream, positioned at the beginning.
        /// </param>
        /// <param name="render">
        /// The diagram render (SVG) byte stream, positioned at the beginning.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A tuple of the created render and source handles. The render is the
        /// artifact authored content displays; the source is what the editor
        /// reopens for edit.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the current principal is granted neither media-management
        /// nor content-authoring access on the owning page (share-based,
        /// Application-layer gate).
        /// </exception>
        Task<(WikiMediaReadDto Render, WikiMediaReadDto Source)> StoreDiagramAsync(
            WikiMediaWriteDto metadata,
            Stream source,
            Stream render,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves the editable <b>source</b> (mxfile) handle for a stored
        /// diagram <b>render</b> so the editor can reopen it for edit (ADR-018,
        /// §10 of the body-storage implementation note).
        /// </summary>
        /// <remarks>
        /// Loads the render handle, applies the same share-based read gate as
        /// <see cref="GetMediaBytesAsync"/>, and follows its
        /// <see cref="WikiMediaWriteDto.SourceMediaFK"/> to the source handle.
        /// Returns <c>null</c> when the render does not exist, when access is
        /// denied (deliberately indistinguishable so the endpoint is never an
        /// existence oracle), or when the render carries no source link (e.g. a
        /// plain media handle). The returned handle's bytes are fetched with the
        /// existing <see cref="GetMediaBytesAsync"/> using the source id.
        /// </remarks>
        /// <param name="renderMediaId">
        /// The id of the render <c>WikiMedia</c> handle (the SVG the
        /// <c>drawio:{id}</c> token points at).
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The linked source media handle, or <c>null</c> if the render does not
        /// exist, access is denied, or there is no linked source.
        /// </returns>
        Task<WikiMediaReadDto?> GetDiagramSourceAsync(
            Guid renderMediaId,
            CancellationToken cancellationToken = default);
    }
}
