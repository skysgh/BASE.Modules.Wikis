using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Sys.Application.Base;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.AccessControl.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations
{
    /// <summary>
    /// Application service implementation for <c>WikiTemplate</c> CRUST operations
    /// (ADR-018C templates-as-pages).
    /// </summary>
    /// <remarks>
    /// Uses <see cref="SimpleCrustStateAppServiceBase{TEntity,TDto}"/> to provide
    /// standard create/read/update/state-transition orchestration. Mapping is handled
    /// via the injected <see cref="IObjectMappingService"/>; AutoMapper configuration
    /// must register <c>WikiTemplate ↔ WikiTemplateDto</c> bidirectional maps.
    /// </remarks>
    public class WikiTemplateAppService
        : SimpleCrustStateAppServiceBase<WikiTemplate, WikiTemplateDto>,
          IWikiTemplateAppService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiTemplateAppService"/> class.
        /// </summary>
        /// <param name="repository">CRUST repository for template persistence.</param>
        /// <param name="objectMappingService">Object mapping service for entity ↔ DTO projection.</param>
        /// <param name="loggingService">Logger instance for diagnostics.</param>
        /// <param name="applicationAuthorizationService">Authorization service (optional for now).</param>
        public WikiTemplateAppService(
            IWikiTemplateRepository repository,
            IObjectMappingService objectMappingService,
            IAppLogger loggingService,
            IApplicationAuthorizationService? applicationAuthorizationService = null)
            : base(repository, objectMappingService, loggingService, applicationAuthorizationService)
        {
        }
    }
}
