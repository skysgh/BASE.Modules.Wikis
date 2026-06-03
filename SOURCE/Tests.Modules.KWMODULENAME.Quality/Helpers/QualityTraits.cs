namespace Tests.Modules.KWMODULENAME.Quality.Helpers
{
	/// <summary>
	/// ISO-25010 quality attribute trait constants for categorising quality proof tests.
	/// <para>
	/// Usage: <c>[Trait(QualityTraits.Category, QualityTraits.Iso25010.Reliability.Maturity)]</c>
	/// </para>
	/// <para>
	/// Filter: <c>dotnet test --filter "Quality=Reliability.Maturity"</c>
	/// </para>
	/// </summary>
	public static class QualityTraits
	{
		/// <summary>
		/// The trait key used for quality attribute classifications.
		/// </summary>
		public const string Category = "Quality";

		/// <summary>
		/// ISO-25010 Product Quality Model characteristics.
		/// </summary>
		public static class Iso25010
		{
			/// <summary>
			/// Functional Suitability: degree to which the product provides
			/// functions that meet stated and implied needs.
			/// </summary>
			public static class FunctionalSuitability
			{
				/// <summary>Degree to which functions cover all specified tasks and user objectives.</summary>
				public const string Completeness = "FunctionalSuitability.Completeness";

				/// <summary>Degree to which functions provide correct results with needed precision.</summary>
				public const string Correctness = "FunctionalSuitability.Correctness";
			}

			/// <summary>
			/// Reliability: degree to which a system performs specified functions
			/// under specified conditions for a specified period of time.
			/// </summary>
			public static class Reliability
			{
				/// <summary>Degree to which a system meets needs for reliability under normal operation.</summary>
				public const string Maturity = "Reliability.Maturity";
			}

			/// <summary>
			/// Maintainability: degree of effectiveness and efficiency with which
			/// a product can be modified.
			/// </summary>
			public static class Maintainability
			{
				/// <summary>Degree to which a system is composed of discrete components.</summary>
				public const string Modularity = "Maintainability.Modularity";

				/// <summary>Degree of effectiveness and efficiency with which it is possible to assess impact of a change.</summary>
				public const string Analysability = "Maintainability.Analysability";
			}
		}
	}
}
