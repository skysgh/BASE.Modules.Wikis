namespace Tests.Modules.Wikis.Dynamic.Helpers
{
	/// <summary>
	/// Trait name constants for categorising Wikis module tests.
	/// <para>
	/// Every test should carry at least a <see cref="Mode"/> and a
	/// <see cref="Capability"/> trait; add a <see cref="Quality"/> trait
	/// whenever the test addresses an ISO-25010 quality characteristic.
	/// </para>
	/// <para>
	/// Usage: <c>[Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]</c>
	/// </para>
	/// <para>
	/// Filters:
	/// <c>dotnet test --filter "Mode=Static"</c>,
	/// <c>dotnet test --filter "Capability=Media"</c>,
	/// <c>dotnet test --filter "Quality=Reliability.Maturity"</c>
	/// </para>
	/// <para>
	/// NOTE: an identical copy of this file exists in
	/// <c>Tests.Modules.Wikis.Static</c>. Keep the two in sync.
	/// </para>
	/// </summary>
	public static class TestTraits
	{
		/// <summary>
		/// Trait key for the execution mode of the test.
		/// </summary>
		public const string Mode = "Mode";

		/// <summary>
		/// Execution modes. Mirrors the two test assemblies per module.
		/// </summary>
		public static class Modes
		{
			/// <summary>Runs fully in-process: no host, no network, no real database.</summary>
			public const string Static = "Static";

			/// <summary>Requires a running host, real infrastructure, or real database.</summary>
			public const string Dynamic = "Dynamic";
		}

		/// <summary>
		/// Trait key for the functional capability under test.
		/// Values are module-specific; extend <see cref="Capabilities"/> per module.
		/// </summary>
		public const string Capability = "Capability";

		/// <summary>
		/// Functional capabilities of this module. Extend as the module grows.
		/// </summary>
		public static class Capabilities
		{
			/// <summary>Wiki pages, versioning, and ACL scopes.</summary>
			public const string Pages = "Pages";

			/// <summary>Wiki templates and their structure.</summary>
			public const string Templates = "Templates";

			/// <summary>Wiki media items, byte transfer, and diagrams.</summary>
			public const string Media = "Media";

			/// <summary>Body content hashing, storage paths, and storage sinks.</summary>
			public const string BodyStorage = "BodyStorage";

			/// <summary>Access authorization services and gates.</summary>
			public const string Authorization = "Authorization";

			/// <summary>Module configuration section naming and defaults.</summary>
			public const string Configuration = "Configuration";

			/// <summary>Module-wide structural and DI conventions.</summary>
			public const string Conventions = "Conventions";
		}

		/// <summary>
		/// Trait key for ISO-25010 quality attribute classifications.
		/// </summary>
		public const string Quality = "Quality";

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

				/// <summary>Degree to which functions facilitate the accomplishment of specified tasks.</summary>
				public const string Appropriateness = "FunctionalSuitability.Appropriateness";
			}

			/// <summary>
			/// Performance Efficiency: performance relative to resources used.
			/// </summary>
			public static class PerformanceEfficiency
			{
				/// <summary>Response and processing times and throughput rates.</summary>
				public const string TimeBehaviour = "PerformanceEfficiency.TimeBehaviour";

				/// <summary>Amounts and types of resources used when performing functions.</summary>
				public const string ResourceUtilization = "PerformanceEfficiency.ResourceUtilization";

				/// <summary>Degree to which maximum limits of a parameter are met.</summary>
				public const string Capacity = "PerformanceEfficiency.Capacity";
			}

			/// <summary>
			/// Compatibility: ability to exchange information and coexist with other systems.
			/// </summary>
			public static class Compatibility
			{
				/// <summary>Ability to perform required functions while sharing environment and resources.</summary>
				public const string CoExistence = "Compatibility.CoExistence";

				/// <summary>Ability to exchange information and use exchanged information.</summary>
				public const string Interoperability = "Compatibility.Interoperability";
			}

			/// <summary>
			/// Usability: effectiveness, efficiency and satisfaction of use.
			/// </summary>
			public static class Usability
			{
				/// <summary>Degree to which users can learn to use the product.</summary>
				public const string Learnability = "Usability.Learnability";

				/// <summary>Degree to which the product is easy to operate and control.</summary>
				public const string Operability = "Usability.Operability";

				/// <summary>Degree to which the product protects users against making errors.</summary>
				public const string UserErrorProtection = "Usability.UserErrorProtection";

				/// <summary>Degree to which the product can be used by people with the widest range of characteristics.</summary>
				public const string Accessibility = "Usability.Accessibility";
			}

			/// <summary>
			/// Reliability: degree to which a system performs specified functions
			/// under specified conditions for a specified period of time.
			/// </summary>
			public static class Reliability
			{
				/// <summary>Degree to which a system meets needs for reliability under normal operation.</summary>
				public const string Maturity = "Reliability.Maturity";

				/// <summary>Degree to which a system is operational and accessible when required.</summary>
				public const string Availability = "Reliability.Availability";

				/// <summary>Degree to which a system operates as intended despite faults.</summary>
				public const string FaultTolerance = "Reliability.FaultTolerance";

				/// <summary>Degree to which data and state can be recovered after an interruption or failure.</summary>
				public const string Recoverability = "Reliability.Recoverability";
			}

			/// <summary>
			/// Security: degree to which the product protects information and data.
			/// </summary>
			public static class Security
			{
				/// <summary>Degree to which data is accessible only to those authorised.</summary>
				public const string Confidentiality = "Security.Confidentiality";

				/// <summary>Degree to which data and programs are protected from unauthorised modification.</summary>
				public const string Integrity = "Security.Integrity";

				/// <summary>Degree to which actions can be proven to have taken place.</summary>
				public const string NonRepudiation = "Security.NonRepudiation";

				/// <summary>Degree to which actions of an entity can be traced uniquely to that entity.</summary>
				public const string Accountability = "Security.Accountability";

				/// <summary>Degree to which the identity of a subject or resource can be proved.</summary>
				public const string Authenticity = "Security.Authenticity";
			}

			/// <summary>
			/// Maintainability: degree of effectiveness and efficiency with which
			/// a product can be modified.
			/// </summary>
			public static class Maintainability
			{
				/// <summary>Degree to which a system is composed of discrete components.</summary>
				public const string Modularity = "Maintainability.Modularity";

				/// <summary>Degree to which an asset can be used in more than one system.</summary>
				public const string Reusability = "Maintainability.Reusability";

				/// <summary>Degree of effectiveness and efficiency with which it is possible to assess impact of a change.</summary>
				public const string Analysability = "Maintainability.Analysability";

				/// <summary>Degree to which a product can be modified without introducing defects.</summary>
				public const string Modifiability = "Maintainability.Modifiability";

				/// <summary>Degree of effectiveness and efficiency with which test criteria can be established and performed.</summary>
				public const string Testability = "Maintainability.Testability";
			}

			/// <summary>
			/// Portability: degree to which a product can be transferred between environments.
			/// </summary>
			public static class Portability
			{
				/// <summary>Degree to which a product can be adapted to different environments.</summary>
				public const string Adaptability = "Portability.Adaptability";

				/// <summary>Degree of effectiveness and efficiency of installation and uninstallation.</summary>
				public const string Installability = "Portability.Installability";

				/// <summary>Degree to which a product can replace another for the same purpose.</summary>
				public const string Replaceability = "Portability.Replaceability";
			}
		}
	}
}
