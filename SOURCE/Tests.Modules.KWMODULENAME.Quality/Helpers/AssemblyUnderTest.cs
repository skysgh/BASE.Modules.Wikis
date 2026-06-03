using System.Reflection;

namespace Tests.Modules.KWMODULENAME.Quality.Helpers
{
	/// <summary>
	/// Provides reflection-based access to all KWMODULENAME module assemblies
	/// for quality proof tests.
	/// <para>
	/// Caches assembly metadata to avoid repeated loading.
	/// All quality tests use this to scan types, methods, and attributes.
	/// </para>
	/// </summary>
	public static class AssemblyUnderTest
	{
		/// <summary>
		/// All KWMODULENAME module production assemblies (non-test).
		/// </summary>
		public static IReadOnlyList<Assembly> AllAssemblies { get; } = LoadAll();

		/// <summary>
		/// Assemblies that belong to the Application layer.
		/// </summary>
		public static IReadOnlyList<Assembly> ApplicationAssemblies { get; } =
			AllAssemblies.Where(a => GetName(a).Contains(".Application")).ToList();

		/// <summary>
		/// Assemblies that belong to the Domain layer.
		/// </summary>
		public static IReadOnlyList<Assembly> DomainAssemblies { get; } =
			AllAssemblies.Where(a => GetName(a).Contains(".Domain")).ToList();

		/// <summary>
		/// Assemblies that belong to the Infrastructure layer.
		/// </summary>
		public static IReadOnlyList<Assembly> InfrastructureAssemblies { get; } =
			AllAssemblies.Where(a => GetName(a).Contains(".Infrastructure")).ToList();

		/// <summary>
		/// Assemblies that belong to the Interface layer (REST, OData, GraphQL, Web).
		/// </summary>
		public static IReadOnlyList<Assembly> InterfaceAssemblies { get; } =
			AllAssemblies.Where(a => GetName(a).Contains(".Interfaces")).ToList();

		/// <summary>
		/// All public types (including interfaces and abstract) across all assemblies.
		/// </summary>
		public static IReadOnlyList<Type> AllPublicTypes { get; } =
			AllAssemblies
				.SelectMany(SafeGetTypes)
				.Where(t => t.IsPublic)
				.ToList();

		/// <summary>
		/// All types (public and non-public) across all module assemblies.
		/// Required for verifying internal implementations.
		/// </summary>
		public static IReadOnlyList<Type> AllTypes { get; } =
			AllAssemblies
				.SelectMany(SafeGetTypes)
				.ToList();

		/// <summary>
		/// All concrete (non-abstract, non-interface) types across all assemblies.
		/// Includes internal types.
		/// </summary>
		public static IReadOnlyList<Type> AllConcreteTypes { get; } =
			AllTypes
				.Where(t => !t.IsAbstract && !t.IsInterface)
				.ToList();

		/// <summary>
		/// All enum types across all assemblies.
		/// </summary>
		public static IReadOnlyList<Type> AllEnums { get; } =
			AllAssemblies
				.SelectMany(SafeGetTypes)
				.Where(t => t.IsEnum && !t.IsNested)
				.Distinct()
				.ToList();

		private static string GetName(Assembly a)
		{
			return a.GetName().Name ?? string.Empty;
		}

		private static List<Assembly> LoadAll()
		{
			// Use the Substrate's disk-based discovery to load ALL App.*.dll,
			// then filter to this module. This ensures transitive dependencies
			// are available and mirrors the Host's startup behaviour.
			App.AssemblyDiscoveryExtensions.PreloadModuleAssembliesFromDisk();

			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(a =>
				{
					var name = a.GetName().Name ?? string.Empty;
					return name.Contains("App.Modules.KWMODULENAME", StringComparison.OrdinalIgnoreCase)
						&& !name.Contains("Tests", StringComparison.OrdinalIgnoreCase);
				})
				.OrderBy(a => a.GetName().Name)
				.ToList();
		}

		/// <summary>
		/// Safe type loader that handles <see cref="ReflectionTypeLoadException"/>.
		/// </summary>
		public static Type[] SafeGetTypes(Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				return ex.Types.Where(t => t != null).ToArray()!;
			}
		}
	}
}
