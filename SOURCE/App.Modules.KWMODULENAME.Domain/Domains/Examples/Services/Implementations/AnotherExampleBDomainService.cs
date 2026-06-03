using System;
using App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations;
using App.Modules.KWMODULENAME.Domain.Domains.Examples.Constants;
using App.Modules.Sys.Shared.Services.Attributes;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Services.Implementations
{
    /// <summary>
    /// Alternate configuration-selected implementation of <see cref="IAnotherExampleDomainService"/>.
    /// </summary>
    [ConditionalService(KWMODULENAMEConfigKeys.Implementation, KWMODULENAMEConfigKeys.ImplementationOptions.AnotherExampleB)]
    public class AnotherExampleBDomainService : IAnotherExampleBDomainService
    {
        private readonly AnotherExampleAServiceConfiguration _exampleServiceConfiguration;
        private readonly ExampleConfigurationObject _exampleConfigurationObject;

        /// <summary>
        /// Initialises a new instance of the <see cref="AnotherExampleBDomainService"/> class.
        /// </summary>
        /// <param name="exampleServiceConfigurationObject">Bound example configuration for the template module.</param>
        /// <param name="exampleConfigurationObject"></param>
        public AnotherExampleBDomainService(
            AnotherExampleAServiceConfiguration exampleServiceConfigurationObject,
            ExampleConfigurationObject exampleConfigurationObject)
        {
            ArgumentNullException.ThrowIfNull(exampleServiceConfigurationObject);

            this._exampleServiceConfiguration = exampleServiceConfigurationObject;
            this._exampleConfigurationObject = exampleConfigurationObject;
        }

        /// <inheritdoc />
        public string Hello()
        {
            return $"Hello from {this._exampleConfigurationObject.HelloFrom} via implementation B.";
        }

        /// <inheritdoc />
        public bool AreYouTryingToDoGood()
        {
            return this._exampleConfigurationObject.AreYouTryingToDoGood;
        }
    }
}