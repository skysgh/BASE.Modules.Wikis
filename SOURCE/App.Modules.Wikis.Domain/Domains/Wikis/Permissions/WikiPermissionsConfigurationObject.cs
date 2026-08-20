using App.Modules.Wikis;
using App.Modules.Sys.Shared.Permissions.Attributes;
using App.Modules.Sys.Shared.Permissions.Models;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Permissions
{
    /// <summary>
    /// Permission-definition container for the Wikis module.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The discoverable type defines the ownership boundary; the nested
    /// constants are the true logical permission identifiers and carry their
    /// metadata via <see cref="PermissionDescriptionAttribute"/>. This is the
    /// production replacement for the template example permissions container.
    /// </para>
    /// <para>
    /// Permission keys follow the <c>{module}/{domain}/{action}</c> shape and
    /// distinguish reading content, authoring/versioning pages, managing media,
    /// administering share-based access, and configuring module behaviour.
    /// </para>
    /// </remarks>
    public class WikiPermissionsConfigurationObject : IPermissionsGroup
    {
        /// <summary>
        /// Permissions governing wiki content and administration.
        /// </summary>
        public static class Permissions
        {
            private const string Module = ModuleConstants.Key;
            private const string Domain = Module + "/Wikis";

            /// <summary>
            /// Read/view permission for wiki pages and their rendered content.
            /// </summary>
            [PermissionDescription(
                "Wiki read",
                "Allow reading and viewing wiki pages and their content.",
                Grouping = ModuleConstants.Name + ";Wikis;Content")]
            public const string Read = Domain + "/Read";

            /// <summary>
            /// Author permission for creating and editing wiki pages
            /// (produces new immutable page versions).
            /// </summary>
            [PermissionDescription(
                "Wiki author",
                "Allow creating and editing wiki pages, producing new page versions.",
                Grouping = ModuleConstants.Name + ";Wikis;Content")]
            public const string Author = Domain + "/Author";

            /// <summary>
            /// Manage permission for uploading and curating wiki media handles.
            /// </summary>
            [PermissionDescription(
                "Wiki manage media",
                "Allow uploading and managing wiki media.",
                Grouping = ModuleConstants.Name + ";Wikis;Media")]
            public const string ManageMedia = Domain + "/ManageMedia";

            /// <summary>
            /// Administer permission for share-based access-control entries on a
            /// wiki (granting/revoking principal access).
            /// </summary>
            [PermissionDescription(
                "Wiki administer access",
                "Allow managing share-based access-control entries on a wiki.",
                Grouping = ModuleConstants.Name + ";Wikis;Access")]
            public const string AdministerAccess = Domain + "/AdministerAccess";

            /// <summary>
            /// Configure permission for the module behaviour settings exposed by
            /// the wiki configuration object.
            /// </summary>
            [PermissionDescription(
                "Wiki configure",
                "Allow modifying wiki module behaviour configuration values.",
                Grouping = ModuleConstants.Name + ";Wikis;Configuration")]
            public const string Configure = Domain + "/Configure";
        }
    }
}
