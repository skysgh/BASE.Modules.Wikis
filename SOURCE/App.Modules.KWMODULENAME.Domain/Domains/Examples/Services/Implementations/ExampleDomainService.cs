using App.Modules.Sys.Shared.Domains.Services.Implementations;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Services.Implementations
{
    /// <summary>
    /// Example implementation of a normal domain service.
    /// </summary>
    /// <remarks>
    /// Keep domain services focused on business rules that do not sit naturally on a single entity.
    /// Application services should orchestrate. Domain services should decide.
    /// </remarks>
    public class ExampleDomainService : DomainServiceBase, IExampleDomainService
    {
        /// <inheritdoc />
        public string GetExampleMessage()
        {
            return "Example domain service invoked.";
        }
    }
}
