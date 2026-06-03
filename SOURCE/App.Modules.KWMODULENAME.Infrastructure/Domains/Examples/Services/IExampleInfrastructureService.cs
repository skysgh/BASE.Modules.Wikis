using App.Modules.Sys.Shared.Domains.Infrastructure;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Services
{
    /// <summary>
    /// Infrastructure service placeholder for external dependency wrappers.
    /// Infrastructure services wrap third-party libraries, external APIs,
    /// file systems, caches, etc. — isolating the rest of the codebase.
    /// </summary>
    /// <remarks>
    /// Only infrastructure assemblies should reference third-party packages.
    /// The rest of the code depends on app-specific interfaces defined in Shared or Domain.
    /// </remarks>
    public interface IExampleInfrastructureService : IHasInfrastructureSingletonService
    {
    }
}