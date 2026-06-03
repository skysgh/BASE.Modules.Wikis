using System;
using App.Modules.KWMODULENAME.Domain.Domains.Examples.Configuration.Implementations;
using App.Modules.KWMODULENAME.Domain.Domains.Examples.Constants;
using App.Modules.Sys.Shared.Services.Attributes;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Services.Implementations
{
    /// <summary>
    /// Configuration-selected implementation of <see cref="IAnotherExampleDomainService"/>.
    /// </summary>
    /// <remarks>
    /// The conditional attribute shows how reflection-based registration can choose between
    /// multiple implementations without introducing magic strings into the registration path.
    /// </remarks>
    [ConditionalService(KWMODULENAMEConfigKeys.Implementation, KWMODULENAMEConfigKeys.ImplementationOptions.AnotherExampleA)]
    public class AnotherExampleADomainService : IAnotherExampleADomainService
    {
        private readonly AnotherExampleAServiceConfiguration _exampleServiceConfiguration;
        private readonly ExampleConfigurationObject _exampleConfigurationObject;

        /// <summary>
        /// Initialises a new instance of the <see cref="AnotherExampleADomainService"/> class.
        /// </summary>
        /// <param name="exampleServiceConfigurationObject">Bound example configuration for the template module.</param>
        /// <param name="exampleConfigurationObject"></param>
        public AnotherExampleADomainService(
            AnotherExampleAServiceConfiguration exampleServiceConfigurationObject,
            ExampleConfigurationObject exampleConfigurationObject)
        {
            this._exampleServiceConfiguration = exampleServiceConfigurationObject;
            this._exampleConfigurationObject = exampleConfigurationObject;
        }

        /// <inheritdoc />
        public string Hello()
        {
            return $"Hello from {this._exampleConfigurationObject.HelloFrom}.";
        }

        /// <inheritdoc />
        public bool AreYouTryingToDoGood()
        {
            return this._exampleConfigurationObject.AreYouTryingToDoGood;
        }
    }
}