using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Application.Base;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IWikiAclApplicationService"/>.
    /// </summary>
    public class WikiAclApplicationService
        : CrustStateAppServiceBase<WikiAcl, WikiAclReadDto, WikiAclWriteDto, WikiAclWriteDto>,
          IWikiAclApplicationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiAclApplicationService"/> class.
        /// </summary>
        /// <param name="repository">The WikiAcl repository for CRUST persistence.</param>
        /// <param name="mapper">The object mapping service for ProjectTo and Map.</param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        public WikiAclApplicationService(
            ICrustStateRepository<WikiAcl> repository,
            IObjectMappingService mapper,
            IAppLogger logger)
            : base(repository, mapper, logger)
        {
        }
    }
}
