using App.Modules.KWMODULENAME;
using App.Modules.Sys.Shared.Permissions.Attributes;
using App.Modules.Sys.Shared.Permissions.Models;

namespace App.Modules.KWMODULENAME.Domain.Domains.Examples.Permissions
{
    /// <summary>
    /// Example permission-definition container for the Template logical module.
    /// </summary>
    /// <remarks>
    /// This demonstrates the proposed container-based direction for permissions:
    /// the discoverable type defines the ownership boundary, while the constants inside it
    /// remain the true logical identifiers and carry permission metadata via attributes.
    /// </remarks>
    public class ExamplePermissionsConfigurationObject : IPermissionsConfigurationObject
    {
        private const string Module = ModuleConstants.Key;
        private const string Domain = Module + ".Examples";

        /// <summary>
        /// Access permission for the Template examples surface.
        /// </summary>
        [PermissionDefinition(
            "Template examples access",
            "Allow access to the Template module examples surface.",
            Grouping = ModuleConstants.Key + ";Examples;Examples")]
        public const string Access = Domain + ".Access";

        /// <summary>
        /// View permission for the Template examples surface.
        /// </summary>
        [PermissionDefinition(
            "Template examples view",
            "Allow viewing Template module example content.",
            Grouping = ModuleConstants.Key + ";Examples;Examples")]
        public const string View = Domain + ".View";

        /// <summary>
        /// Configure permission for the nested example service configuration.
        /// </summary>
        [PermissionDefinition(
            "Template examples configure",
            "Allow modifying Template example configuration values.",
            Grouping = ModuleConstants.Key + ";Examples;Configuration")]
        public const string Configure = Domain + ".Configure";
    }
}

