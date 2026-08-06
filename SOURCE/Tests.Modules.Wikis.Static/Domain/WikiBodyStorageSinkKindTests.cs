using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using Tests.Modules.Wikis.Static.Helpers;

namespace Tests.Modules.Wikis.Static.Domain
{
    /// <summary>
    /// Gate tests for <see cref="WikiBodyStorageSinkKind"/> (ADR-018N): the enum
    /// must follow the framework reserved-sentinel convention (first four members
    /// reserved, real options from 4), and must expose exactly the three sinks the
    /// ADR defines. Persisted-config enums that drift here would silently change
    /// which sink an environment selects, so the values are pinned.
    /// </summary>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.BodyStorage)]
    [Trait(TestTraits.Quality, TestTraits.Iso25010.FunctionalSuitability.Correctness)]
    public class WikiBodyStorageSinkKindTests
    {
        [Fact]
        public void WhenInspectingSentinels_ThenTheyFollowTheFrameworkEnumConvention()
        {
            Assert.Equal(0, (int)WikiBodyStorageSinkKind.Undefined);
            Assert.Equal(1, (int)WikiBodyStorageSinkKind.NotApplicable);
            Assert.Equal(2, (int)WikiBodyStorageSinkKind.Unspecified);
            Assert.Equal(3, (int)WikiBodyStorageSinkKind.Unknown);
        }

        [Fact]
        public void WhenInspectingRealOptions_ThenTheyBeginAtFourAndAreTheThreeSinks()
        {
            Assert.Equal(4, (int)WikiBodyStorageSinkKind.Database);
            Assert.Equal(5, (int)WikiBodyStorageSinkKind.ObjectStore);
            Assert.Equal(6, (int)WikiBodyStorageSinkKind.FileSystem);
        }

        [Fact]
        public void WhenCountingRealOptions_ThenThereAreExactlyThree()
        {
            int realOptionCount = 0;
            foreach (WikiBodyStorageSinkKind value in Enum.GetValues<WikiBodyStorageSinkKind>())
            {
                if ((int)value >= 4)
                {
                    realOptionCount++;
                }
            }

            Assert.Equal(3, realOptionCount);
        }
    }
}
