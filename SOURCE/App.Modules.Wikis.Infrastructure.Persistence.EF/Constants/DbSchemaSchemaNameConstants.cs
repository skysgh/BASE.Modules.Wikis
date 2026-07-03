namespace App.Modules.Wikis.Infrastructure.Constants
{
	/// <summary>
	/// Database schema name constants for organizing tables into logical groups.
	/// </summary>
	public static class DbSchemaSchemaNameConstants
	{
		/// <summary>
		/// Default schema for this module (same as module key).
		/// </summary>
		public const string Root = App.Modules.Wikis.ModuleConstants.DbSchemaKey;

		/// <summary>
		/// Schema for the core wiki domain tables (ADR-018).
		/// </summary>
		public const string Wikis = Root + "_wikis";

		/// <summary>
		/// Schema for reference/lookup data tables.
		/// </summary>
		public const string ReferenceData = Root + "_ref";
	}
}
