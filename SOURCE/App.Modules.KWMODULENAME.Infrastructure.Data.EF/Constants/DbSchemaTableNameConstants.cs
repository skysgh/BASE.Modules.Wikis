namespace App.Modules.KWMODULENAME.Infrastructure.Constants
{
	/// <summary>
	/// Database table name constants for organizing tables into logical groups
	/// while avoiding reliance on magic strings.
	/// </summary>
	public static class DbSchemaTableNameConstants
	{
		/// <summary>
		/// Table of Example A entities.
		/// </summary>
		public const string ExampleAs = "ExampleAs";

		/// <summary>
		/// Table of Example B entities.
		/// </summary>
		public const string ExampleBs = "ExampleBs";

		/// <summary>
		/// Table of ExampleType reference-data entities.
		/// </summary>
		public const string ExampleTypes = "ExampleTypes";

		/// <summary>
		/// Table of ExampleValueObject entities (children of ExampleType).
		/// </summary>
		public const string ExampleValueObjects = "ExampleValueObjects";

		/// <summary>
		/// Join table linking ExampleA and ExampleB (many-to-many).
		/// </summary>
		public const string ExampleAExampleBs = "ExampleAExampleBs";
	}
}
