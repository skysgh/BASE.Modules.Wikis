using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.Sys.Shared.Application;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Services
{
    /// <summary>
    /// Application service contract for <see cref="Shared.Domains.Examples.Models.Implmentations.ExampleType"/> CRUST operations.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This is an Application Service contract, not a domain service contract.
    /// Note that this service inherits from
    /// <see cref="ICrudStateAppService{TEntityDto,TCreateDto,TUpdateDto}"/>
    /// This is the common pattern for the vaste majority of Application Services contracts in the system,
    /// as it provides a standard set of CRUD operations with
    /// state management following our IQueryable-based repository patterns.
    /// </remarks>
    public interface IExampleTypeApplicationService
		: ICrudStateAppService<ExampleTypeDto, ExampleTypeDto, ExampleTypeDto>
	{
	}
}
