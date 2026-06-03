using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Application.Base;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Repositories;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Services.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IExampleAApplicationService"/>.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This is an Application Service contract, not a domain service contract.
    /// Note that this service inherits from
    /// <see cref="SimpleCrustStateAppServiceBase{TEntity,TEntityDto}"/>
    /// This is the common pattern for the vaste majority of Application Services contracts in the system,
    /// as it provides a standard set of CRUD operations with
    /// state management following our IQueryable-based repository patterns.
    /// </remarks>
    public class ExampleAApplicationService
		: SimpleCrustStateAppServiceBase<ExampleA, ExampleADto>,
		  IExampleAApplicationService
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleAApplicationService"/> class.
        /// </summary>
        /// <param name="repository">The ExampleA repository for CRUST persistence.</param>
        /// <param name="mapper">The object mapping service for ProjectTo and Map.</param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        /// <remarks>
        /// IMPORTANT:
        /// Note how it is injected with the <see cref="ICrustStateRepository{TEntity}"/>
        /// for CRUST persistence, in a queryable way.
        /// Note also, that it is injected with the <see cref="IObjectMappingService"/> for ProjectTo and Map,
        /// which the 'secret sauce' to make IQUyerable work across the projections from DTO to entities.
        /// </remarks>  
        public ExampleAApplicationService(
			ICrustStateRepository<ExampleA> repository,
			IObjectMappingService mapper,
			IAppLogger logger)
			: base(repository, mapper, logger)
		{
		}
	}
}
