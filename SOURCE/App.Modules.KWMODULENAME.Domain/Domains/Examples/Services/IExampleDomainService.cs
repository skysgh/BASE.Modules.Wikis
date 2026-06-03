using App.Modules.Sys.Shared.Domains.Services;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Services
{
    /// <summary>
    /// Domain service contract for example business rules.
    /// </summary>
    /// <remarks>
    /// Most modules will have thin domain services. Complex business logic goes here when it spans
    /// multiple entities or requires coordination. Application services orchestrate; domain services encapsulate rules.
    /// </remarks>
    public interface IExampleDomainService : IHasDomainScopedService
    {
        /// <summary>
        /// Returns a short example message to show a normal domain service method.
        /// </summary>
        /// <returns>An example message from the template service.</returns>
        string GetExampleMessage();
    }
}
