using App.Modules.Sys.Shared.Application;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Application service contract for <see cref="WikiPageReadDto"/> operations.
    /// Extends <see cref="ICrudStateAppService{TReadDto,TCreateDto,TUpdateDto}"/>
    /// for standard CRUST operations, returning IQueryable for OData filtering,
    /// paging, and sorting at the API boundary.
    /// </summary>
    public interface IWikiPageApplicationService
        : ICrudStateAppService<WikiPageReadDto, WikiPageWriteDto, WikiPageWriteDto>
    {
        /// <summary>
        /// Composes the server-side single-GET render projection for a page by
        /// its id: page addressing/metadata plus the current published version's
        /// metadata and inline body text (resolved through the body store).
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The composed <see cref="WikiPageContentReadDto"/>, or <c>null</c> when
        /// no page exists with that id. A page that exists but has no published
        /// version is returned with
        /// <see cref="WikiPageContentReadDto.HasContent"/> = <c>false</c>.
        /// </returns>
        Task<WikiPageContentReadDto?> GetContentByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Composes the server-side single-GET render projection for a page by
        /// its canonical <see cref="WikiPageContentReadDto.Path"/> within a wiki
        /// root (DokuWiki-style addressing).
        /// </summary>
        /// <param name="wikiId">The owning wiki root id.</param>
        /// <param name="path">
        /// The canonical full namespace path (e.g.
        /// <c>engineering/onboarding/setup</c>).
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The composed <see cref="WikiPageContentReadDto"/>, or <c>null</c> when
        /// no page exists at that path. A path whose page exists but has no
        /// published version is returned with
        /// <see cref="WikiPageContentReadDto.HasContent"/> = <c>false</c>, which
        /// the client renders as the "create this page" invitation.
        /// </returns>
        Task<WikiPageContentReadDto?> GetContentByPathAsync(
            Guid wikiId,
            string path,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists the thin page rows that belong to a single wiki root,
        /// ordered by their canonical <c>Path</c> so the
        /// client can build a stable tree without depending on generic OData
        /// filtering or ordering over internal foreign-key properties.
        /// </summary>
        /// <param name="wikiId">The owning wiki root id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The ordered page rows for the wiki root.</returns>
        Task<IReadOnlyList<WikiPageReadDto>> GetPagesByWikiAsync(
            Guid wikiId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves an edit to a page's content: stores the submitted body through
        /// the body store, appends a new immutable
        /// <see cref="WikiPageContentReadDto.CurrentVersionId">version</see>, and
        /// repoints the page's current pointer. When no page exists at the
        /// requested <see cref="WikiPageContentWriteDto.WikiFK"/> +
        /// <see cref="WikiPageContentWriteDto.Path"/> the page is created first
        /// (DokuWiki-style "create this page").
        /// </summary>
        /// <param name="request">The content to save (addressing + body).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// The composed <see cref="WikiPageContentReadDto"/> for the saved page,
        /// reflecting the newly published version and its inline body. Never
        /// <c>null</c> on success.
        /// </returns>
        Task<WikiPageContentReadDto> SaveContentAsync(
            WikiPageContentWriteDto request,
            CancellationToken cancellationToken = default);
    }
}
