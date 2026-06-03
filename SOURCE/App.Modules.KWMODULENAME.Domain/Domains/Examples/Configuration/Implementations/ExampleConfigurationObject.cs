using App.Modules.KWMODULENAME.Domain.Domains.Examples.Constants;
using App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations;
using App.Modules.Sys.Shared.Attributes;
using App.Modules.Sys.Shared.Domains.Lifecycles;
using App.Modules.Sys.Shared.Domains.Configuration.Attributes;
using App.Modules.Sys.Domains.Configurations.Models;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations
{
    /// <summary>
    /// Example module-level configuration section for the template logical module.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is intentionally small. It exists to show how a module can expose a dedicated
    /// configuration section that drives service behaviour without introducing magic strings.
    /// </para>
    /// <para>
        /// Note that this inherits from <see cref="IConfigurationObject"/>
    /// which inherits from <see cref="IHasSingletonLifecycle"/>.
    /// </para>
        /// <para>
        /// This type also demonstrates that a module-level <see cref="IConfigurationObject"/> can contain
        /// a nested <see cref="IAnotherExampleServiceConfiguration"/> subtree used to configure one injected service.
        /// </para>
    /// </remarks>
    [Alias(KWMODULENAMEConfigKeys.Examples)]
    public class ExampleConfigurationObject : IConfigurationObject
    {
        /// <summary>
        /// Configuration section path for the example settings.
        /// </summary>
        public const string SectionPath = KWMODULENAMEConfigKeys.Examples;

        /// <summary>
        /// Gets or sets the source text used by the example greeting method.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            false,
            KWMODULENAMEConfigKeys.HelloFrom,
            "Hello From",
            "Example configuration value used by the template greeting method.",
            ModuleConstants.Name)]
        public string HelloFrom { get; set; } = ModuleConstants.Name;

        /// <summary>
        /// Gets or sets a simple boolean value used by the example service.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            false,
            KWMODULENAMEConfigKeys.AreYouTryingToDoGood,
            "Are You Trying To Do Good",
            "Example boolean value used to demonstrate a configuration-backed service decision.",
            "true")]
        public bool AreYouTryingToDoGood { get; set; } = true;

        /// <summary>
        /// Gets or sets the selected nested service configuration implementation metadata.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            false,
            KWMODULENAMEConfigKeys.AnotherExampleServiceImplementation,
            "Another Example Service Implementation",
            "Identifies which nested service configuration implementation should be used.",
            KWMODULENAMEConfigKeys.ImplementationOptions.AnotherExampleA)]
        public string AnotherExampleServiceImplementation { get; set; } = KWMODULENAMEConfigKeys.ImplementationOptions.AnotherExampleA;

        /// <summary>
        /// Gets or sets the nested configuration subtree for implementation A.
        /// </summary>
        public AnotherExampleAServiceConfiguration ServiceA { get; set; } = new AnotherExampleAServiceConfiguration();

        /// <summary>
        /// Gets or sets the nested configuration subtree for implementation B.
        /// </summary>
        public AnotherExampleBServiceConfiguration ServiceB { get; set; } = new AnotherExampleBServiceConfiguration();
    }
}
