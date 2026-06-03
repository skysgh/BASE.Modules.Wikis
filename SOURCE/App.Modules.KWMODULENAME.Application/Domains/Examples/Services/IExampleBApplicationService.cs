using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.Sys.Shared.Application;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Services
{
    /// <summary>
    /// Application service contract for ExampleB operations.
    /// Extends <see cref="ICrudStateAppService{TReadDto,TCreateDto,TUpdateDto}"/>
    /// for standard CRUST operations.
    /// Returns IQueryable for OData filtering, paging, sorting at the API boundary.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This is an Application Service contract, not a domain service contract.
    /// Note that this service inherits from
    /// <see cref="ICrudStateAppService{TEntityDto,TCreateDto,TUpdateDto}"/>
    /// This is the common pattern for the vaste majority of Application Services contracts in the system,
    /// as it provides a standard set of CRUD operations with
    /// state management following our IQueryable-based repository patterns.
    /// </remarks>
    public interface IExampleBApplicationService
		: ICrudStateAppService<ExampleBDto, ExampleBDto, ExampleBDto>
	{
	}
}
