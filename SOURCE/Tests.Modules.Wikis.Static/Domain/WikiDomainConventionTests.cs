using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using Tests.Modules.Wikis.Static.Helpers;

namespace Tests.Modules.Wikis.Static.Domain
{
    /// <summary>
    /// Phase-A gate tests for the wiki domain's fixed defaults and the
    /// folder-index resolution enum. These lock in the framework enum
    /// convention (reserved sentinels first, real values from 4) and the
    /// shipped defaults that the configuration object seeds from, so the
    /// shipped literal and the admin default never drift apart.
    /// </summary>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.Conventions)]
    [Trait(TestTraits.Quality, TestTraits.Iso25010.FunctionalSuitability.Correctness)]
    public class WikiDomainConventionTests
    {
        [Fact]
        public void WhenInspectingFolderIndexResolutionMode_ThenItFollowsTheReservedSentinelConvention()
        {
            Assert.Equal(0, (int)FolderIndexResolutionMode.Undefined);
            Assert.Equal(1, (int)FolderIndexResolutionMode.NotApplicable);
            Assert.Equal(2, (int)FolderIndexResolutionMode.Unspecified);
            Assert.Equal(3, (int)FolderIndexResolutionMode.Unknown);

            // Real options begin at 4 per the framework enum convention.
            Assert.Equal(4, (int)FolderIndexResolutionMode.IndexThenPage);
            Assert.Equal(5, (int)FolderIndexResolutionMode.PageThenIndex);
            Assert.Equal(6, (int)FolderIndexResolutionMode.IndexOnly);
            Assert.Equal(7, (int)FolderIndexResolutionMode.PageOnly);
        }

        [Fact]
        public void WhenInspectingShippedDefaults_ThenTheRootDocumentNameIsFormatNeutralHome()
        {
            // The root document slug stays extension-free; the extension is
            // implied by the content-format parser.
            Assert.Equal("home", WikiDomainConstants.DefaultRootDocumentName);
            Assert.DoesNotContain(".", WikiDomainConstants.DefaultRootDocumentName);
        }

        [Fact]
        public void WhenInspectingCrossLinkTokenParts_ThenTheySupportTheWikiKeySlugShape()
        {
            // wiki:{key}:{slug}
            string token = string.Join(
                WikiDomainConstants.CrossLinkSeparator,
                WikiDomainConstants.CrossLinkScheme,
                "handbook",
                "getting-started");

            Assert.Equal("wiki:handbook:getting-started", token);
            Assert.Equal("wiki", WikiDomainConstants.CrossLinkScheme);
            Assert.Equal("/", WikiDomainConstants.PathSegmentSeparator);
        }
    }
}
