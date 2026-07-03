using System;
using App.Modules.Wikis;
using App.Modules.Wikis.Infrastructure.Persistence.EF;
using App.Modules.Sys.Initialisation.Implementation.Base;
using Microsoft.Extensions.DependencyInjection;

namespace App.Modules.Wikis.AppEntry
{
    /// <summary>
    /// Module assembly initialiser for the Wikis logical module.
    /// </summary>
        /// <remarks>
        /// This hook exists to show that modules can participate in the startup lifecycle before and after
        /// the root service provider is built. It should remain the exception, not the normal registration path.
        /// Reflection-based service discovery is the default and preferred approach, including for
        /// this module's section-bound configuration objects.
        /// </remarks>
    public class ModuleAssemblyInitialiser : ModuleAssemblyInitialiserBase
    {
        /// <summary>
        /// Registers module-specific startup prerequisites before the service provider is built.
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

            // Register this module's DbContext via the shared helper (ADR-006).
            services.AddModuleDbContext<ModuleDbContext>(ModuleConstants.DbSchemaKey);
        }

        /// <summary>
        /// Allows post-build module startup participation after the service provider is built.
        /// </summary>
        /// <param name="serviceProvider">Built root service provider.</param>
        public override void DoAfterBuild(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
        }
    }
}
