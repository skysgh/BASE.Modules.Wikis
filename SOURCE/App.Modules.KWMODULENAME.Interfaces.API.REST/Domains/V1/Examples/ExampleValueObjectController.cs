using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Application.Domains.Examples.Services;
using App.Modules.KWMODULENAME.Interfaces.API.REST.Constants;
using App.Modules.Sys.Controllers;
using App.Modules.Sys.Interfaces.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules.KWMODULENAME.Interfaces.API.REST.Domains.V1.Examples
{
    /// <summary>
    /// REST controller for <see cref="ExampleValueObjectDto"/> CRUST operations.
    /// Provides: GET all, GET by id, POST, PUT, PATCH state, PATCH record-state.
    /// </summary>
    /// <remarks>
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
    [Route(ApiRoutes.Rest.V1.ExampleValueObjects.Base)]
	public class ExampleValueObjectController : SimpleCrudStateControllerBase<ExampleValueObjectDto>, IHasScopedController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExampleValueObjectController"/> class.
		/// </summary>
		public ExampleValueObjectController(IExampleValueObjectApplicationService service)
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
