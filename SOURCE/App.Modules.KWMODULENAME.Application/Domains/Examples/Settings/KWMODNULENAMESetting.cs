using System;
using System.Collections.Generic;
using System.Text;
using App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations;
using App.Modules.Sys.Domains.Configurations.Models;
using App.Modules.Sys.Infrastructure.Domains.Configuration.Models.Implementations;
using App.Modules.Sys.Shared.Definitions;
using App.Modules.Sys.Shared.Domains.Configuration.Attributes;
using App.Modules.Sys.Shared.Services.Configuration;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Settings
{
    /// <summary>
    /// /A Configuration object that is
    /// discoverable at startup, due to
    /// inherititng from <see cref="IDiscoverableDefinitionObject"/>
    /// which in turn inherits from <c>IHasSingletonLifecycle</c>
    /// </summary>
    /// <remarks>
    /// Often has nested objects that derive from
    /// <see cref="IServiceConfiguration"/>
    /// </remarks>
    public class KWMODNULENAMESettings : IConfigurationObject
    {
        internal static class Paths {
            public const string A = "";
        }

        // Can ontain nested objects that derive from IServiceH

        // properties that will be display are decoratew tih an attribute

        [ConfigurationPropertyDescriptionAttribute(true, true, Paths.A, "Example", "Example setting.")]
        public bool Example { get; set; }


        /// <summary>
        /// Can Reference nested objects
        /// </summary>
        public ExampleConfigurationObject ExampleSub {get;set;}= new ExampleConfigurationObject();

    }
}
