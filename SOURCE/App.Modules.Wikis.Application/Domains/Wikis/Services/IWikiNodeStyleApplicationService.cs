using App.Modules.Sys.Shared.Application;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Application service contract for <see cref="WikiNodeStyleReadDto"/> operations.
    /// </summary>
    public interface IWikiNodeStyleApplicationService
        : ICrudStateAppService<WikiNodeStyleReadDto, WikiNodeStyleWriteDto, WikiNodeStyleWriteDto>
    {
        /// <summary>
        /// Returns all node-style rows that apply to a given page.
        /// </summary>
        /// <param name="wikiPageId">The owning wiki page id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The page's additive style rows.</returns>
        Task<IReadOnlyList<WikiNodeStyleReadDto>> GetForPageAsync(
            Guid wikiPageId,
            CancellationToken cancellationToken = default);
    }
}
