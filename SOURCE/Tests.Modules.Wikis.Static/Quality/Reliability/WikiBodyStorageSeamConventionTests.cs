using System.Reflection;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using Tests.Modules.Wikis.Static.Helpers;
using Tests.Modules.Wikis.Static.Quality.Helpers;

namespace Tests.Modules.Wikis.Static.Quality.Reliability
{
    /// <summary>
    /// Reliability gate for the ADR-018N pluggable wiki body-storage seam: proves
    /// the seam is fully and uniquely wired so an environment's configured sink
    /// (Database / ObjectStore / FileSystem) always resolves to exactly one store,
    /// and the coordinator exists to front them. These are reflection gates so they
    /// cannot drift if a sink is added, renamed, or duplicated.
    /// </summary>
    /// <remarks>
    /// This is the visible signal demanded by the no-stub rule: if a future change
    /// adds a <see cref="WikiBodyStorageSinkKind"/> value without an implementation,
    /// or duplicates a <see cref="IWikiBodyStore.Kind"/>, or removes the
    /// coordinator, the build's test stage fails here rather than at runtime when an
    /// admin flips a config key.
    /// </remarks>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.Conventions)]
    public class WikiBodyStorageSeamConventionTests
    {
        /// <summary>
        /// Every real (non-sentinel) sink kind must have exactly one concrete
        /// <see cref="IWikiBodyStore"/> implementation, so the coordinator's
        /// kind-to-store dispatch is total and unambiguous.
        /// </summary>
        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public void WhenInspectingSinks_ThenEachRealKindHasExactlyOneImplementation()
        {
            List<IWikiBodyStore> sinks = InstantiateAllSinks();

            foreach (WikiBodyStorageSinkKind kind in RealSinkKinds())
            {
                int count = sinks.Count(s => s.Kind == kind);
                Assert.True(
                    count == 1,
                    $"Expected exactly one IWikiBodyStore for sink kind '{kind}', found {count}.");
            }
        }

        /// <summary>
        /// No two sinks may declare the same <see cref="IWikiBodyStore.Kind"/>,
        /// otherwise the coordinator's last-wins dictionary would silently shadow
        /// a sink.
        /// </summary>
        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public void WhenInspectingSinks_ThenNoTwoDeclareTheSameKind()
        {
            List<IWikiBodyStore> sinks = InstantiateAllSinks();

            IEnumerable<IGrouping<WikiBodyStorageSinkKind, IWikiBodyStore>> duplicates =
                sinks.GroupBy(s => s.Kind).Where(g => g.Count() > 1);

            Assert.Empty(duplicates);
        }

        /// <summary>
        /// No sink may advertise a sentinel kind: a sentinel would never be
        /// selected by config and signals a copy-paste or default-init mistake.
        /// </summary>
        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public void WhenInspectingSinks_ThenNoneDeclareASentinelKind()
        {
            List<IWikiBodyStore> sinks = InstantiateAllSinks();

            foreach (IWikiBodyStore sink in sinks)
            {
                Assert.True(
                    (int)sink.Kind >= 4,
                    $"Sink '{sink.GetType().Name}' declares sentinel kind '{sink.Kind}'.");
            }
        }

        /// <summary>
        /// The seam must expose exactly one coordinator implementation; it is the
        /// single entry point the page-save use case depends on.
        /// </summary>
        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public void WhenInspectingCoordinator_ThenExactlyOneImplementationExists()
        {
            List<Type> coordinators = AssemblyUnderTest.AllConcreteTypes
                .Where(t => typeof(IWikiBodyStoreCoordinator).IsAssignableFrom(t))
                .ToList();

            Assert.Single(coordinators);
        }

        /// <summary>
        /// Both seam contracts (single-sink store and coordinator) must be scoped,
        /// because the Database sink binds to a scoped DbContext and the whole
        /// family runs within the page-save use-case scope.
        /// </summary>
        /// <remarks>
        /// The scoped marker is resolved from the seam contract's own inheritance
        /// chain (it extends <c>IHasScopedService</c>, which extends
        /// <c>IHasScopedLifecycle</c>) rather than by scanning Substrate by name,
        /// so the gate cannot silently no-op if Substrate is filtered out of the
        /// module assembly set.
        /// </remarks>
        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public void WhenInspectingSeamContracts_ThenTheyAreScopedLifecycle()
        {
            Type scopedLifecycleMarker = ResolveScopedLifecycleMarkerFromSeam();

            Assert.True(
                scopedLifecycleMarker.IsAssignableFrom(typeof(IWikiBodyStore)),
                "IWikiBodyStore must resolve to a scoped lifecycle.");
            Assert.True(
                scopedLifecycleMarker.IsAssignableFrom(typeof(IWikiBodyStoreCoordinator)),
                "IWikiBodyStoreCoordinator must resolve to a scoped lifecycle.");
        }

        /// <summary>
        /// Locates the <c>IHasScopedLifecycle</c> marker by walking the seam
        /// contract's interface graph, so it is found regardless of which
        /// assembly declares it.
        /// </summary>
        private static Type ResolveScopedLifecycleMarkerFromSeam()
        {
            Type? marker = typeof(IWikiBodyStore)
                .GetInterfaces()
                .FirstOrDefault(i => i.Name == "IHasScopedLifecycle");

            Assert.True(
                marker is not null,
                "IHasScopedLifecycle was not found in IWikiBodyStore's interface graph; "
                + "the seam contract must extend a scoped lifecycle marker.");

            return marker!;
        }

        private static IEnumerable<WikiBodyStorageSinkKind> RealSinkKinds()
        {
            foreach (WikiBodyStorageSinkKind kind in Enum.GetValues<WikiBodyStorageSinkKind>())
            {
                if ((int)kind >= 4)
                {
                    yield return kind;
                }
            }
        }

        /// <summary>
        /// Instantiates every concrete <see cref="IWikiBodyStore"/> with stub
        /// constructor dependencies, purely to read its <see cref="IWikiBodyStore.Kind"/>.
        /// No method is invoked, so the stubs need no behaviour.
        /// </summary>
        private static List<IWikiBodyStore> InstantiateAllSinks()
        {
            List<Type> sinkTypes = AssemblyUnderTest.AllConcreteTypes
                .Where(t => typeof(IWikiBodyStore).IsAssignableFrom(t))
                .ToList();

            Assert.True(
                sinkTypes.Count >= 3,
                $"Expected at least three IWikiBodyStore implementations, found {sinkTypes.Count}.");

            List<IWikiBodyStore> sinks = new List<IWikiBodyStore>();
            foreach (Type sinkType in sinkTypes)
            {
                ConstructorInfo constructor = sinkType
                    .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .First();

                object?[] arguments = constructor
                    .GetParameters()
                    .Select(p => CreateStub(p.ParameterType))
                    .ToArray();

                sinks.Add((IWikiBodyStore)constructor.Invoke(arguments));
            }

            return sinks;
        }

        /// <summary>
        /// Creates a do-nothing proxy for an interface dependency (enough to
        /// construct a sink whose <c>Kind</c> we only want to read), or a default
        /// for value types. Sinks depend only on interfaces, so this suffices.
        /// </summary>
        private static object? CreateStub(Type parameterType)
        {
            if (parameterType.IsInterface)
            {
                return NoOpInterfaceProxy.Create(parameterType);
            }

            return parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null;
        }
    }
}
