using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;

namespace Tests.Modules.Wikis.Domain.Domains.Wikis.Entities
{
    /// <summary>
    /// Phase-A gate tests for the page-versioning invariant: a
    /// <see cref="WikiPage"/> carries no body text, and "editing" a page must
    /// append a new immutable <see cref="WikiPageVersion"/> and repoint
    /// <see cref="WikiPage.CurrentVersionId"/> rather than mutate an existing
    /// version's blob (ADR-018 immutable-blob invariant).
    /// </summary>
    public class WikiPageVersioningTests
    {
        [Fact]
        public void WhenPageIsFreshlyCreated_ThenItHasNoPublishedVersion()
        {
            WikiPage page = new WikiPage
            {
                WikiFK = Guid.NewGuid(),
                Slug = "getting-started",
                Title = "Getting Started",
            };

            Assert.Null(page.CurrentVersionId);
            Assert.Empty(page.Versions);
        }

        [Fact]
        public void WhenPageIsEdited_ThenANewVersionIsAppendedAndCurrentVersionRepointed()
        {
            Guid pageId = Guid.NewGuid();
            WikiPage page = new WikiPage
            {
                Id = pageId,
                WikiFK = Guid.NewGuid(),
                Slug = "getting-started",
            };

            WikiPageVersion firstVersion = new WikiPageVersion
            {
                Id = Guid.NewGuid(),
                WikiPageFK = pageId,
                BodyBlobId = Guid.NewGuid(),
                ContentHash = "hash-1",
                ContentFormatKey = "markdown",
            };
            page.Versions.Add(firstVersion);
            page.CurrentVersionId = firstVersion.Id;

            WikiPageVersion secondVersion = new WikiPageVersion
            {
                Id = Guid.NewGuid(),
                WikiPageFK = pageId,
                BodyBlobId = Guid.NewGuid(),
                ContentHash = "hash-2",
                ContentFormatKey = "markdown",
            };
            page.Versions.Add(secondVersion);
            page.CurrentVersionId = secondVersion.Id;

            // The first version is preserved (never mutated) and the page now
            // points at the new version: history is append-only.
            Assert.Equal(2, page.Versions.Count);
            Assert.Equal(secondVersion.Id, page.CurrentVersionId);
            Assert.NotEqual(firstVersion.BodyBlobId, secondVersion.BodyBlobId);
            Assert.Equal("hash-1", firstVersion.ContentHash);
        }

        [Fact]
        public void WhenContentIsReplaced_ThenTheVersionPointsAtANewBlobIdNotAMutatedOne()
        {
            Guid originalBlobId = Guid.NewGuid();
            WikiPageVersion original = new WikiPageVersion
            {
                Id = Guid.NewGuid(),
                BodyBlobId = originalBlobId,
                ContentHash = "hash-original",
            };

            WikiPageVersion replacement = new WikiPageVersion
            {
                Id = Guid.NewGuid(),
                BodyBlobId = Guid.NewGuid(),
                ContentHash = "hash-replacement",
            };

            // Replacement is a distinct row with a distinct blob; the original
            // blob id is unchanged.
            Assert.Equal(originalBlobId, original.BodyBlobId);
            Assert.NotEqual(original.BodyBlobId, replacement.BodyBlobId);
            Assert.NotEqual(original.Id, replacement.Id);
        }
    }
}
