using System.Reflection;
using Tests.Modules.KWMODULENAME.Quality.Helpers;

namespace Tests.Modules.KWMODULENAME.Quality.Reliability
{
	/// <summary>
	/// ISO-25010 Reliability.Maturity â€” DI Service Convention Enforcement.
	/// <para>
	/// Ensures all Application-layer service interfaces implement
	/// <c>IHasApplicationService</c> (the convention-based DI discovery marker)
	/// and that their implementations carry the correct lifecycle marker.
	/// </para>
	/// <para>
	/// <b>Layer rules enforced:</b>
	/// <list type="bullet">
	///   <item>Application-layer interfaces (<c>I*AppService</c> / <c>I*ApplicationService</c>)
	///         must extend <c>IHasApplicationService</c> (scoped).</item>
	///   <item>Concrete implementations must implement <c>IHasScopedLifecycle</c>.</item>
	///   <item>Every interface must have at least one concrete implementation.</item>
	///   <item>Controllers must not inject interfaces that lack a lifecycle marker.</item>
	///   <item>Implementations must be <c>internal</c>, not <c>public</c>.</item>
	/// </list>
	/// </para>
	/// </summary>
	public class ServiceConventionTests
	{
		private static readonly string[] AppServiceSuffixes = ["AppService", "ApplicationService"];

		private static readonly HashSet<string> KnownFrameworkTypes =
		[
			"ILogger", "IConfiguration", "IOptions", "IHttpContextAccessor",
			"IWebHostEnvironment", "IHostEnvironment", "IServiceProvider",
			"IMediator", "IMapper", "IObjectMappingService",
		];

		private static readonly Type? AppServiceMarker = FindMarkerType("IHasApplicationService");
		private static readonly Type? ScopedLifecycleMarker = FindMarkerType("IHasScopedLifecycle");
		private static readonly Type? LifecycleMarker = FindMarkerType("IHasLifecycle");

		/// <summary>
		/// Application-layer service interfaces must extend IHasApplicationService
		/// to be discoverable by the DI scanner.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.Reliability.Maturity)]
		public void WhenAppServiceInterfaceDeclaredThenImplementsIHasApplicationService()
		{
			if (AppServiceMarker == null)
			{
				Assert.Fail("IHasApplicationService not found. Ensure Substrate is referenced.");
				return;
			}

			var violations = AssemblyUnderTest.ApplicationAssemblies
				.SelectMany(AssemblyUnderTest.SafeGetTypes)
				.Where(t => t.IsInterface && t.IsPublic)
				.Where(t => IsAppServiceName(t.Name))
				.Where(iface => !AppServiceMarker.IsAssignableFrom(iface))
				.Select(iface => iface.FullName ?? iface.Name)
				.ToList();

			Assert.True(
				violations.Count == 0,
				$"Application service interfaces without IHasApplicationService " +
				$"({violations.Count}). Invisible to DI scanner:\n  " +
				string.Join("\n  ", violations));
		}

		/// <summary>
		/// Application service implementations must implement IHasScopedLifecycle.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.Reliability.Maturity)]
		public void WhenAppServiceImplementationExistsThenImplementsIHasScopedLifecycle()
		{
			if (AppServiceMarker == null || ScopedLifecycleMarker == null)
			{
				Assert.Fail("Marker types not found. Ensure Substrate is referenced.");
				return;
			}

			var violations = AssemblyUnderTest.AllTypes
				.Where(t => !t.IsAbstract && !t.IsInterface)
				.Where(t => t.GetInterfaces().Any(i => AppServiceMarker.IsAssignableFrom(i)))
				.Where(impl => !ScopedLifecycleMarker.IsAssignableFrom(impl))
				.Select(impl => impl.FullName ?? impl.Name)
				.ToList();

			Assert.True(
				violations.Count == 0,
				$"App service implementations without IHasScopedLifecycle " +
				$"({violations.Count}). Wrong DI lifetime:\n  " +
				string.Join("\n  ", violations));
		}

		/// <summary>
		/// Every IHasApplicationService interface must have at least one implementation.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.FunctionalSuitability.Completeness)]
		public void WhenAppServiceInterfaceDeclaredThenHasConcreteImplementation()
		{
			if (AppServiceMarker == null)
			{
				Assert.Fail("IHasApplicationService not found. Ensure Substrate is referenced.");
				return;
			}

			var markerNames = new HashSet<string> { "IHasScopedService", "IHasApplicationService" };

			var appServiceInterfaces = AssemblyUnderTest.ApplicationAssemblies
				.SelectMany(AssemblyUnderTest.SafeGetTypes)
				.Where(t => t.IsInterface && t.IsPublic)
				.Where(AppServiceMarker.IsAssignableFrom)
				.Where(t => t != AppServiceMarker && !markerNames.Contains(t.Name))
				.ToList();

			var allConcreteTypes = AssemblyUnderTest.AllConcreteTypes;

			var unimplemented = appServiceInterfaces
				.Where(iface => !allConcreteTypes.Any(c => iface.IsAssignableFrom(c)))
				.Select(iface => iface.FullName ?? iface.Name)
				.ToList();

			Assert.True(
				unimplemented.Count == 0,
				$"App service interfaces with no implementation ({unimplemented.Count}). " +
				$"DI will throw:\n  " +
				string.Join("\n  ", unimplemented));
		}

		/// <summary>
		/// Controller constructor parameters must be resolvable by DI.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.Reliability.Maturity)]
		public void WhenControllerInjectsInterfaceThenInterfaceIsResolvable()
		{
			if (LifecycleMarker == null)
			{
				Assert.Fail("IHasLifecycle not found. Ensure Substrate is referenced.");
				return;
			}

			var controllerTypes = AssemblyUnderTest.InterfaceAssemblies
				.SelectMany(AssemblyUnderTest.SafeGetTypes)
				.Where(t => !t.IsAbstract && IsControllerType(t))
				.ToList();

			var violations = new List<string>();

			foreach (var controller in controllerTypes)
			{
				foreach (var ctor in controller.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
				{
					foreach (var param in ctor.GetParameters())
					{
						if (!param.ParameterType.IsInterface)
						{
							continue;
						}

						if (IsKnownFrameworkType(param.ParameterType))
						{
							continue;
						}

						if (!LifecycleMarker.IsAssignableFrom(param.ParameterType))
						{
							violations.Add(
								$"{controller.Name} â†’ {param.ParameterType.Name} (no IHasLifecycle)");
						}
					}
				}
			}

			Assert.True(
				violations.Count == 0,
				$"Controllers inject non-resolvable interfaces ({violations.Count}):\n  " +
				string.Join("\n  ", violations));
		}

		/// <summary>
		/// Application service implementations must be internal, not public.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.Maintainability.Modularity)]
		public void WhenAppServiceImplementationExistsThenIsNotPublic()
		{
			if (AppServiceMarker == null)
			{
				Assert.Fail("IHasApplicationService not found. Ensure Substrate is referenced.");
				return;
			}

			var violations = AssemblyUnderTest.AllTypes
				.Where(t => t.IsPublic && !t.IsAbstract && !t.IsInterface)
				.Where(t => t.GetInterfaces().Any(i => AppServiceMarker.IsAssignableFrom(i)))
				.Select(t => t.FullName ?? t.Name)
				.ToList();

			Assert.True(
				violations.Count == 0,
				$"Public app service implementations ({violations.Count}). " +
				$"Should be internal:\n  " +
				string.Join("\n  ", violations));
		}

		// === Helpers ===

		private static bool IsAppServiceName(string name)
		{
			return name.StartsWith('I')
				&& AppServiceSuffixes.Any(s => name.EndsWith(s, StringComparison.Ordinal));
		}

		private static bool IsControllerType(Type type)
		{
			var current = type.BaseType;
			while (current != null)
			{
				if (current.Name is "ControllerBase" or "Controller")
				{
					return true;
				}

				current = current.BaseType;
			}

			return false;
		}

		private static bool IsKnownFrameworkType(Type paramType)
		{
			var name = paramType.IsGenericType
				? paramType.Name[..paramType.Name.IndexOf('`')]
				: paramType.Name;
			return KnownFrameworkTypes.Contains(name);
		}

		private static Type? FindMarkerType(string interfaceName)
		{
			var substrateAssembly = AppDomain.CurrentDomain.GetAssemblies()
				.FirstOrDefault(a =>
				{
					var n = a.GetName().Name;
					return n != null
						&& n.Contains("Substrate", StringComparison.OrdinalIgnoreCase)
						&& n.Contains("Sys", StringComparison.OrdinalIgnoreCase);
				});

			if (substrateAssembly == null)
			{
				App.AssemblyDiscoveryExtensions.PreloadModuleAssembliesFromDisk();
				substrateAssembly = AppDomain.CurrentDomain.GetAssemblies()
					.FirstOrDefault(a =>
					{
						var n = a.GetName().Name;
						return n != null
							&& n.Contains("Substrate", StringComparison.OrdinalIgnoreCase)
							&& n.Contains("Sys", StringComparison.OrdinalIgnoreCase);
					});
			}

			if (substrateAssembly == null)
			{
				return null;
			}

			try
			{
				return substrateAssembly.GetTypes()
					.FirstOrDefault(t => t.IsInterface && t.IsPublic && t.Name == interfaceName);
			}
			catch (ReflectionTypeLoadException ex)
			{
				return ex.Types
					.Where(t => t != null)
					.FirstOrDefault(t => t!.IsInterface && t.IsPublic && t.Name == interfaceName);
			}
		}
	}
}

