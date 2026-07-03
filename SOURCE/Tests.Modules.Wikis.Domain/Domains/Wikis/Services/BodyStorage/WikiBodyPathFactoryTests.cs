using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;

namespace Tests.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage
{
    /// <summary>
    /// Gate tests for <see cref="WikiBodyPathFactory"/> (ADR-018N §2.2/§2.6): the
    /// body path is always <b>derived</b> from immutable inputs, never stored, so
    /// a page move can never strand a path. These tests pin the two path shapes
    /// (object store and content repo), the separation from the media prefix, and
    /// the empty-locator guard.
    /// </summary>
    public class WikiBodyPathFactoryTests
    {
        [Fact]
        public void WhenBuildingObjectStorePath_ThenItUsesTheBodyPrefixDistinctFromMedia()
        {
            Guid locator = Guid.NewGuid();

            string path = WikiBodyPathFactory.BuildObjectStorePath(locator);

            // Bodies live under the dedicated body prefix, never the media prefix,
            // so a body and a media asset can never collide in the store.
            Assert.StartsWith(WikiDomainConstants.BodyBlobPathPrefix, path);
            Assert.DoesNotContain(WikiDomainConstants.MediaBlobPathPrefix, path);
            Assert.Contains(locator.ToString("D"), path);
        }

        [Fact]
        public void WhenBuildingObjectStorePath_ThenTheSameLocatorYieldsTheSamePath()
        {
            Guid locator = Guid.NewGuid();

            string first = WikiBodyPathFactory.BuildObjectStorePath(locator);
            string second = WikiBodyPathFactory.BuildObjectStorePath(locator);

            Assert.Equal(first, second);
        }

        [Fact]
        public void WhenBuildingObjectStorePathWithEmptyLocator_ThenItThrows()
        {
            Assert.Throws<ArgumentException>(() => WikiBodyPathFactory.BuildObjectStorePath(Guid.Empty));
        }

        [Fact]
        public void WhenBuildingFileSystemPath_ThenItIsHumanReadableAndVersioned()
        {
            string path = WikiBodyPathFactory.BuildFileSystemRelativePath("docs", "a/b/c", 3);

            // Human/Git-friendly: prefix, wiki key, slug, and a v{n} leaf.
            Assert.StartsWith(WikiDomainConstants.FileSystemBodyPathPrefix, path);
            Assert.Contains("docs", path);
            Assert.Contains("a/b/c", path);
            Assert.Contains("v3", path);
        }

        [Fact]
        public void WhenBuildingFileSystemPathForDifferentVersions_ThenPathsDiffer()
        {
            string v1 = WikiBodyPathFactory.BuildFileSystemRelativePath("docs", "a/b", 1);
            string v2 = WikiBodyPathFactory.BuildFileSystemRelativePath("docs", "a/b", 2);

            Assert.NotEqual(v1, v2);
        }

        [Fact]
        public void WhenBuildingFileSystemPathWithBlankWikiKey_ThenItThrows()
        {
            Assert.Throws<ArgumentException>(
                () => WikiBodyPathFactory.BuildFileSystemRelativePath(" ", "a/b", 1));
        }

        [Fact]
        public void WhenBuildingFileSystemPathWithNonPositiveVersion_ThenItThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => WikiBodyPathFactory.BuildFileSystemRelativePath("docs", "a/b", 0));
        }
    }
}
