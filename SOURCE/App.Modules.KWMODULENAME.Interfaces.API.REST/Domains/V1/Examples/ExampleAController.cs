using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Application.Domains.Examples.Services;
using App.Modules.KWMODULENAME.Interfaces.API.REST.Constants;
using App.Modules.Sys.Interfaces.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules.KWMODULENAME.Interfaces.API.REST.Domains.V1.Examples
{
    /// <summary>
    /// REST API controller for ExampleA operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Inherits standard CRUST endpoints from
    /// <see cref="SimpleCrudStateControllerBase{TDto}"/>:
    /// <list type="bullet">
    ///   <item><c>GET</c> - all entities with OData query support.</item>
    ///   <item><c>GET {id}</c> - single entity by identifier.</item>
    ///   <item><c>GET by-key/{key}</c> - single entity by unique key.</item>
    ///   <item><c>POST</c> - create a new entity.</item>
    ///   <item><c>PUT {id}</c> - update an existing entity.</item>
    ///   <item><c>PATCH {id}/state</c> - transition domain state.</item>
    ///   <item><c>PATCH {id}/record-state</c> - transition persistence record state.</item>
    /// </list>
    /// </para>
    /// <para>
    /// OData provides filtering by active status, title, etc.
    /// Global middleware enforces MaxTop - no unbounded queries.
    /// </para>
    /// <para>
    /// IMPORTANT: It is *essential*
    /// that Routes and authentication/authorisation
    /// is not using 'magic strings', but
    /// only using Constants.
    /// These constants are defined in this assembly...
    /// extending constants defined in this LM's Shared project,
    /// extending constants defined in App.Modules.Sys.Shared assembly
    /// </para>
    /// <para>
    /// IMPORTANT:
    /// Controllers *must* inherit from
    /// <see cref="SimpleCrudStateControllerBase{TEntityDto}"/>
    /// or the slightly more complex
    /// <see cref="CrudStateControllerBase{TReadDto, TCreateDto, TUpdateDto}"/>
    /// </para>
    /// </remarks>
    [Route(ApiRoutes.Rest.V1.ExampleAs.Base)]
	public class ExampleAController : SimpleCrudStateControllerBase<ExampleADto>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExampleAController"/> class.
		/// </summary>
		/// <param name="service">The ExampleA application service.</param>
		public ExampleAController(IExampleAApplicationService service)
			: base(service)
		{
            // IMPORTANT:
            // Security is not done using
            // the simplistic role managed
            // attributes that .NET provides.
            // Instead it is handled in the
            // base controller, using the
            // service's GetPolicyNameForAction method
            // which puts to effect the system's Share based authorization system, which is much more flexible and powerful than the simplistic role-based system provided by .NET,
            // and is a key part of the system's security model.
        }
    }
}
