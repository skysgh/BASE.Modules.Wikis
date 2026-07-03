using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Application.Base;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Infrastructure.Services.Contracts;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Configuration.Implementations;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IWikiApplicationService"/>.
    /// </summary>
    public class WikiApplicationService
        : CrustStateAppServiceBase<Wiki, WikiReadDto, WikiWriteDto, WikiWriteDto>,
          IWikiApplicationService
    {
        private readonly IAppConfiguration<WikiConfigurationObject> _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiApplicationService"/> class.
        /// </summary>
        /// <param name="repository">The Wiki repository for CRUST persistence.</param>
        /// <param name="mapper">The object mapping service for ProjectTo and Map.</param>
        /// <param name="configuration">The wiki configuration object.</param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        public WikiApplicationService(
            ICrustStateRepository<Wiki> repository,
            IObjectMappingService mapper,
            IAppConfiguration<WikiConfigurationObject> configuration,
            IAppLogger logger)
            : base(repository, mapper, logger)
        {
            this._configuration = configuration;
        }

        /// <inheritdoc />
        public WikiClientConfigReadDto GetClientConfig()
        {
            WikiConfigurationObject config = this._configuration.GetValueOrDefault();

            return new WikiClientConfigReadDto
            {
                DefaultHostNamespace = config.DefaultHostNamespace,
                DefaultHostSlug = config.DefaultHostSlug,
                DefaultRootDocumentName = config.DefaultRootDocumentName,
                NoContentMessage = config.NoContentMessage,
            };
        }
    }
}
