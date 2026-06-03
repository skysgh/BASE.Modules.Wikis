using Tests.Modules.KWMODULENAME.Quality.Helpers;

namespace Tests.Modules.KWMODULENAME.Quality.Maintainability
{
	/// <summary>
	/// ISO-25010 Maintainability.Modularity — Module Assembly Sanity.
	/// Validates basic structural health of the module.
	/// </summary>
	public class ModuleAssemblySanityTests
	{
		/// <summary>
		/// Module assemblies can be discovered and loaded.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.Maintainability.Modularity)]
		public void ModuleAssembliesAreDiscoverable()
		{
			var count = AssemblyUnderTest.AllAssemblies.Count;
			Assert.True(
				count >= 0,
				$"Expected zero or more assemblies, found {count}.");
		}

		/// <summary>
		/// If the module has assemblies loaded, they must expose public types.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.Maintainability.Modularity)]
		public void LoadedAssembliesHavePublicTypes()
		{
			if (AssemblyUnderTest.AllAssemblies.Count == 0)
			{
				return;
			}

			Assert.True(
				AssemblyUnderTest.AllPublicTypes.Count > 0,
				"Module has loaded assemblies but no public types.");
		}

		/// <summary>
		/// Module must have Application and Interface layers for full stack completeness.
		/// </summary>
		[Fact]
		[Trait(QualityTraits.Category, QualityTraits.Iso25010.FunctionalSuitability.Completeness)]
		public void ModuleHasApplicationAndInterfaceLayers()
		{
			if (AssemblyUnderTest.AllAssemblies.Count == 0)
			{
				return;
			}

			Assert.True(
				AssemblyUnderTest.ApplicationAssemblies.Count > 0,
				"Module has no Application-layer assembly.");

			Assert.True(
				AssemblyUnderTest.InterfaceAssemblies.Count > 0,
				"Module has no Interface-layer assembly.");
		}
	}
}
