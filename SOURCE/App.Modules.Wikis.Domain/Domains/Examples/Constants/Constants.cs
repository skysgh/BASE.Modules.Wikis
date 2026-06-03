using App.Modules.Wikis;
using App.Modules.Sys.Shared.Constants;

namespace App.Modules.Wikis.Domain.Domains.Examples.Constants
{
    /// <summary>
    /// Configuration keys used by the template example services.
    /// </summary>
    public static class WikisConfigKeys
    {
        /// <summary>
        /// Module configuration section name.
        /// <para>
        /// <c>Wikis</c>
        /// </para>
        /// </summary>
        public const string Name = ModuleConstants.Name;

        /// <summary>
        /// Configuration root for the module.
        /// <para>
        /// <c>App:Domains:Wikis</c>
        /// </para>
        /// </summary>
        public const string Root = $"{ServiceConfigKeys.ServicesRoot}:{Name}";

        /// <summary>
        /// Configuration section containing example settings for the template module.
        /// <para>
        /// <c>App:Domains:Wikis:Examples</c>
        /// </para>
        /// </summary>
        public const string Examples = $"{Root}:Examples";

        /// <summary>
        /// Configuration key for choosing which implementation satisfies
        /// <c>IAnotherExampleDomainService</c>.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:Implementation</c>
        /// </para>
        /// </summary>
        public const string Implementation = $"{Examples}:Implementation";

        /// <summary>
        /// Configuration key for the greeting source value used by the example service.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:HellowFrom</c>
        /// </para>
        /// </summary>
        public const string HelloFrom = $"{Examples}:HelloFrom";

        /// <summary>
        /// Configuration key for the boolean example setting used by the example service.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:AreYouTryingToDoGood</c>
        /// </para>
        /// </summary>
        public const string AreYouTryingToDoGood = $"{Examples}:AreYouTryingToDoGood";

        /// <summary>
        /// Configuration key for the nested service configuration section.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:AnotherExampleService</c>
        /// </para>
        /// </summary>
        public const string AnotherExampleService = $"{Examples}:AnotherExampleService";

        /// <summary>
        /// Configuration key for selecting the nested service implementation configuration.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:AnotherExampleService:Implementation</c>
        /// </para>
        /// </summary>
        public const string AnotherExampleServiceImplementation = $"{AnotherExampleService}:Implementation";

        /// <summary>
        /// Configuration key for the shared service URL used by the nested service configuration example.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:AnotherExampleService:ServiceUrl</c>
        /// </para>
        /// </summary>
        public const string AnotherExampleServiceUrl = $"{AnotherExampleService}:ServiceUrl";

        /// <summary>
        /// Configuration key for the service A specific nested configuration value.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:AnotherExampleService:ServiceA:SomethingSpecific</c>
        /// </para>
        /// </summary>
        public const string SomethingSpecificToServiceA = $"{AnotherExampleService}:ServiceA:SomethingSpecific";

        /// <summary>
        /// Configuration key for the service B specific nested configuration value.
        /// <para>
        /// <c>App:Domains:Wikis:Examples:AnotherExampleService:ServiceB:SomethingSpecific</c>
        /// </para>
        /// </summary>
        public const string SomethingSpecificToServiceB = $"{AnotherExampleService}:ServiceB:SomethingSpecific";

        /// <summary>
        /// Known implementation options for the conditional service example.
        /// </summary>
        public static class ImplementationOptions
        {
            /// <summary>
            /// No conditional implementation selected.
            /// </summary>
            public const string None = "None";

            /// <summary>
            /// Implementation A of <c>IAnotherExampleDomainService</c>.
            /// </summary>
            public const string AnotherExampleA = "AnotherExampleA";

            /// <summary>
            /// Implementation B of <c>IAnotherExampleDomainService</c>.
            /// </summary>
            public const string AnotherExampleB = "AnotherExampleB";
        }
    }
}
