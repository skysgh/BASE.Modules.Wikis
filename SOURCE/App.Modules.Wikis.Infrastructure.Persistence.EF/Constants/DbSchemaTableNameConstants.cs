namespace App.Modules.Wikis.Infrastructure.Constants
{
	/// <summary>
	/// Database table name constants for organizing tables into logical groups
	/// while avoiding reliance on magic strings.
	/// </summary>
	public static class DbSchemaTableNameConstants
	{
		/// <summary>
		/// Table of <c>Wiki</c> roots (mountable wiki spaces).
		/// </summary>
		public const string Wikis = "Wikis";

		/// <summary>
		/// Table of <c>WikiPage</c> entities (the page tree).
		/// </summary>
		public const string WikiPages = "WikiPages";

		/// <summary>
		/// Table of immutable <c>WikiPageVersion</c> snapshots.
		/// </summary>
		public const string WikiPageVersions = "WikiPageVersions";

		/// <summary>
		/// Table of <c>WikiPageVersionBody</c> rows holding version body text for
		/// the Database body-storage sink (ADR-018N). 1:1 with a version; present
		/// only when the Database sink is active for that body.
		/// </summary>
		public const string WikiPageVersionBodies = "WikiPageVersionBodies";

		/// <summary>
		/// Table of <c>WikiMedia</c> immutable media blob handles.
		/// </summary>
		public const string WikiMedia = "WikiMedia";

		/// <summary>
		/// Table of <c>WikiAcl</c> share-based access-control entries.
		/// </summary>
		public const string WikiAcls = "WikiAcls";

		/// <summary>
		/// Table of <c>WikiTemplate</c> reusable page scaffolds (ADR-018C).
		/// </summary>
		public const string WikiTemplates = "WikiTemplates";

		/// <summary>
		/// Table of <c>WikiTemplateSection</c> ordered scaffold/lint sections.
		/// </summary>
		public const string WikiTemplateSections = "WikiTemplateSections";

		/// <summary>
		/// Table of <c>WikiTemplateBinding</c> namespace/subtree template bindings.
		/// </summary>
		public const string WikiTemplateBindings = "WikiTemplateBindings";

		/// <summary>
		/// Table of <c>WikiNodeStyle</c> reference-data rows used by the client
		/// render pipeline to map author-declared section background keys to
		/// allowlisted presentation.
		/// </summary>
		public const string WikiNodeStyles = "WikiNodeStyles";
	}
}
