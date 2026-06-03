using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Shared.Repositories;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="ExampleType"/> reference-data persistence.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This is a Domain Repository contract, not an Application Service contract.
    /// Extends <see cref="ICrustStateRepository{TEntity}"/> for standard CRUST
    /// (Create, Read, Update, State-Transition) persistence operations.
    /// <para>
    /// It will be implemented within Infrastructure.Data.EF,
    /// and injected into the Application Service contract
    /// </para>
    /// </remarks>  
    public interface IExampleTypeRepository : ICrustStateRepository<ExampleType>
	{
	}
}
