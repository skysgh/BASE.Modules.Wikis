using App.Modules.KWMODULENAME.Domain.Domains.Examples.Constants;
using App.Modules.Sys.Shared.Attributes;
using App.Modules.Sys.Shared.Domains.Configuration.Attributes;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations
{
    /// <summary>
    /// Service A implementation-specific configuration subtree.
    /// </summary>
    /// <remarks>
    /// This type exists to show that an <see cref="App.Modules.Sys.Shared.Services.Configuration.IServiceConfiguration"/> can expose shared members through
    /// an interface while also adding implementation-specific members beneath the same module configuration section.
    /// </remarks>
    public class AnotherExampleAServiceConfiguration : IAnotherExampleServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the shared service URL.
        /// </summary>
        public string ServiceUrl { get; set; } = "https://example.com/api";

        /// <summary>
        /// Gets or sets a setting specific to implementation A.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            false,
            KWMODULENAMEConfigKeys.SomethingSpecificToServiceA,
            "Service A Specific Value",
            "Example nested configuration value that only applies to implementation A.",
            "ServiceA specific value")]
        public string SomethingSpecificToServiceA { get; set; } = "ServiceA specific value";
    }
}
