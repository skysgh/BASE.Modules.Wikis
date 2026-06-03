using App.Modules.Sys.Infrastructure.Domains.Constants;

namespace App.Modules.KWMODULENAME
{
    /// <summary>
    /// Constants for the KWMODULENAME module configuration.
    /// </summary>
    public class ModuleConstants
    {
        /// <summary>
        /// Unique dipsplayable Name to
        /// identify this logical module.
        /// </summary>
        public const string Name = "KWMODULENAME";

        /// <summary>
        /// Unique Lowercase Key to use as name for DbSchema 
        /// and api route fragment for this Logical Module.
        /// </summary>
        public const string Key = "kwmodulename";
        /// <summary>
        /// Database schema key for this module.
        /// Used as the default schema prefix in EF configurations.
        /// </summary>
        public const string DbSchemaKey = Key;


        /// <summary>
        /// The display name of the module.
        /// </summary>
        public const string Title = Name;

        /// <summary>
        /// The description of the module.
        /// </summary>
        public const string Description = "TODO: Describe.";


        /// <summary>
        /// The name of the ConnectionString
        /// in app settings
        /// (it's the same as the Base one).
        /// </summary>
        public const string DbConnectionName = AppConstants.DbConnectionStringKey;
    }
}