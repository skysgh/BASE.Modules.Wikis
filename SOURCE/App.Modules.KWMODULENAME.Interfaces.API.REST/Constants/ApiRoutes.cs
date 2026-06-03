using App.Modules.Sys.Application.Domains.AccessControl.Permissions.Constants;

namespace App.Modules.KWMODULENAME.Interfaces.API.REST.Constants
{
	/// <summary>
	/// KWMODULENAME module API route constants.
	/// NO MAGIC STRINGS - all routes composed from constants.
	/// Organized by: {root}/{api-type}/{module}/{version}/{path}
	/// </summary>
	/// <remarks>
	/// Pattern: api/rest/kwmodulename/v1/{controller-path}
	/// Built on shared <see cref="ApiConstants"/> from Substrate.
	/// </remarks>
	public static class ApiRoutes
	{
		private const string ModuleId = ModuleConstants.Key;
		private const string RestModuleBase = ApiConstants.Root + "/" + ApiConstants.RestType + "/" + ModuleId;

		/// <summary>
		/// REST API routes for KWMODULENAME module.
		/// </summary>
		public static class Rest
		{
			/// <summary>
			/// Version 1 of KWMODULENAME module REST APIs.
			/// </summary>
			public static class V1
			{
				private const string VersionBase = RestModuleBase + "/" + ApiConstants.Versions.V1;

				/// <summary>
				/// Standard controller route template.
				/// Value: "api/rest/kwmodulename/v1/{controller}"
				/// </summary>
				public const string ControllerRoute = VersionBase + "/{controller}";

				/// <summary>ExampleA endpoints.</summary>
				public static class ExampleAs
				{
					/// <summary>Value: "api/rest/kw/v1/example-a"</summary>
					public const string Base = VersionBase + "/example-a";
				}

				/// <summary>ExampleB endpoints.</summary>
				public static class ExampleBs
				{
					/// <summary>Value: "api/rest/kw/v1/example-b"</summary>
					public const string Base = VersionBase + "/example-b";
				}

				/// <summary>ExampleType reference-data endpoints.</summary>
				public static class ExampleTypes
				{
					/// <summary>Value: "api/rest/kw/v1/example-type"</summary>
					public const string Base = VersionBase + "/example-type";
				}

							/// <summary>ExampleValueObject endpoints.</summary>
								public static class ExampleValueObjects
								{
									/// <summary>Value: "api/rest/kw/v1/example-value-object"</summary>
									public const string Base = VersionBase + "/example-value-object";
								}
							}
						}
					}
				}
