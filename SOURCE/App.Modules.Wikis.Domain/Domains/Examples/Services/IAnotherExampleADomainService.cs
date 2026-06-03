using App.Modules.Wikis.Domain.Domains.Examples.Services.Implementations;

namespace App.Modules.Wikis.Domain.Domains.Examples.Services
{
    /// <summary>
    /// Specialised marker contract for the configuration-selected
    /// <see cref="AnotherExampleADomainService"/> implementation.
    /// </summary>
    public interface IAnotherExampleADomainService : IAnotherExampleDomainService
    {
    }
}