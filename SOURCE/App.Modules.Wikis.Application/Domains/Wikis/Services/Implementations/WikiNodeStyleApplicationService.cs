using App.Modules.Sys.Application.Base;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IWikiNodeStyleApplicationService"/>.
    /// </summary>
    public class WikiNodeStyleApplicationService
        : CrustStateAppServiceBase<WikiNodeStyle, WikiNodeStyleReadDto, WikiNodeStyleWriteDto, WikiNodeStyleWriteDto>,
          IWikiNodeStyleApplicationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiNodeStyleApplicationService"/> class.
        /// </summary>
        /// <param name="repository">The WikiNodeStyle repository for CRUST persistence.</param>
        /// <param name="mapper">The object mapping service for ProjectTo and Map.</param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        public WikiNodeStyleApplicationService(
            ICrustStateRepository<WikiNodeStyle> repository,
            IObjectMappingService mapper,
            IAppLogger logger)
            : base(repository, mapper, logger)
        {
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<WikiNodeStyleReadDto>> GetForPageAsync(
            Guid wikiPageId,
            CancellationToken cancellationToken = default)
        {
            List<WikiNodeStyle> entities = await this.Repository
                .Query()
                .Where(x => x.WikiPageFK == wikiPageId)
                .OrderBy(x => x.SectionKey)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            List<WikiNodeStyleReadDto> dtos = entities
                .Select(entity => this.ObjectMappingService.Map<WikiNodeStyle, WikiNodeStyleReadDto>(entity))
                .ToList();

            return dtos;
        }
    }
}
