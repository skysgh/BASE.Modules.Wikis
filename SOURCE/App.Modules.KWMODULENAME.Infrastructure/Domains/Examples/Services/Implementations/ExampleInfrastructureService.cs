using System;
using System.Collections.Generic;
using System.Text;
using App.Modules.Sys.Shared.Domains.Infrastructure.Implementations;
using App.Modules.Sys.Shared.Domains.Lifecycles;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Services.Implementations
{
    /// <summary>
    /// Implementation of
    /// <see cref="IExampleInfrastructureService"/>
    /// </summary>
    /// <remarks>
    /// The service inherits from the
    /// <see cref="InfrastructureSingletonServiceBase"/>
    /// which inherits from a contract that
    /// inherits from <see cref="IHasLifecycle"/>
    /// which ensures it is discoverable by reflection.
    /// </remarks>
    public class ExampleInfrastructureService:
        InfrastructureSingletonServiceBase,
        IExampleInfrastructureService
    {
        // In 95% of the cases, in a Logical Module,
        // above Sys - the infrastructure service
        // is not needed - they don't usually add
        // new tech - just business logic.
        // All the code - if any will be in either
        // the LM's Domain Project - or in the
        // the LM's Infrastructure.Data.Db.EF project
        // where the repositories, schemas and seeding
        // are implemented.
    }
}
