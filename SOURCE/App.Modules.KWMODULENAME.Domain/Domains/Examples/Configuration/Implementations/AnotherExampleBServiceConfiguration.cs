using App.Modules.KWMODULENAME.Domain.Domains.Examples.Constants;
using App.Modules.Sys.Shared.Attributes;
using App.Modules.Sys.Shared.Domains.Configuration.Attributes;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations
{
    /// <summary>
    /// Service B implementation-specific configuration subtree.
    /// </summary>
    public class AnotherExampleBServiceConfiguration : IAnotherExampleServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the shared service URL.
        /// </summary>
        public string ServiceUrl { get; set; } = "https://example.com/api";

        /// <summary>
        /// Gets or sets a setting specific to implementation B.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            false,
            KWMODULENAMEConfigKeys.SomethingSpecificToServiceB,
            "Service B Specific Value",
            "Example nested configuration value that only applies to implementation B.",
            "ServiceB specific value")]
        public string SomethingSpecificToServiceB { get; set; } = "ServiceB specific value";
    }
}
