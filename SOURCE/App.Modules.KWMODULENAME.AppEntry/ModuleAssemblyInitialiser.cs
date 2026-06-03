using System;
using System.Linq;
using App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations;
using App.Modules.Sys.Initialisation.Implementation.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Modules.KWMODULENAME.AppEntry
{
    /// <summary>
    /// Example module assembly initialiser for the template logical module.
    /// </summary>
    /// <remarks>
    /// This hook exists to show that modules can participate in the startup lifecycle before and after
    /// the root service provider is built. It should remain the exception, not the normal registration path.
    /// Reflection-based service discovery is the default and preferred approach.
    /// </remarks>
    public class ModuleAssemblyInitialiser : ModuleAssemblyInitialiserBase
    {
        /// <summary>
        /// Binds the example configuration object before the service provider is built.
        /// </summary>
        /// <param name="services">Service collection available during startup composition.</param>
        /// <remarks>
        /// Do not use this method for ordinary service implementation registration.
        /// Service registration in BASE is reflection-first and should stay that way.
        /// Only use this hook when there is no reflection-based option, or when startup order forces an exceptional path.
        /// </remarks>
        public override void DoBeforeBuild(IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            ServiceDescriptor? configurationDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IConfiguration));
            IConfiguration? configuration = configurationDescriptor?.ImplementationInstance as IConfiguration;
            if (configuration == null)
            {
                return;
            }

            ExampleConfigurationObject exampleConfigurationObject = new ExampleConfigurationObject();
            configuration.GetSection(ExampleConfigurationObject.SectionPath).Bind(exampleConfigurationObject);
            services.AddSingleton(exampleConfigurationObject);
        }

        /// <summary>
        /// Resolves the example configuration object after the service provider is built.
        /// </summary>
        /// <param name="serviceProvider">Built root service provider.</param>
        public override void DoAfterBuild(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            _ = serviceProvider.GetService<ExampleConfigurationObject>();
        }
    }
}
