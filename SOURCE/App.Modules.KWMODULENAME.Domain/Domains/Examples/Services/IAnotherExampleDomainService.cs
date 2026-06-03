using App.Modules.Sys.Shared.Domains.Services;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Services
{
    /// <summary>
    /// Example contract showing how one common interface may have multiple
    /// implementations, with the active one selected by configuration.
    /// </summary>
    public interface IAnotherExampleDomainService : IHasDomainSingletonService
    {
        /// <summary>
        /// Returns a greeting that uses the configured source value.
        /// </summary>
        /// <returns>A simple greeting string.</returns>
        string Hello();

        /// <summary>
        /// Returns the configured boolean example value.
        /// </summary>
        /// <returns><c>true</c> when the configured value is enabled; otherwise <c>false</c>.</returns>
        bool AreYouTryingToDoGood();
    }
}
