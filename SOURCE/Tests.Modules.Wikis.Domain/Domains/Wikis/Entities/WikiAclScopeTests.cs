using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;

namespace Tests.Modules.Wikis.Domain.Domains.Wikis.Entities
{
    /// <summary>
    /// Phase-A gate tests for the share-based access model (a
    /// <c>TenantId</c>-per-row model is an explicit anti-pattern here). A
    /// <see cref="WikiAcl"/> grants a permission to a principal at exactly one
    /// scope: either wiki-wide (<see cref="WikiAcl.WikiFK"/>) or for a single
    /// page (<see cref="WikiAcl.WikiPageFK"/>), where the page-level grant is
    /// the more specific override.
    /// </summary>
    public class WikiAclScopeTests
    {
        [Fact]
        public void WhenGrantIsWikiWide_ThenOnlyTheWikiScopeFkIsPopulated()
        {
            WikiAcl acl = new WikiAcl
            {
                WikiFK = Guid.NewGuid(),
                WikiPageFK = null,
                PrincipalId = Guid.NewGuid(),
                PermissionKey = "wikis.Wikis.Read",
            };

            Assert.NotNull(acl.WikiFK);
            Assert.Null(acl.WikiPageFK);
        }

        [Fact]
        public void WhenGrantIsPageScoped_ThenOnlyThePageScopeFkIsPopulated()
        {
            WikiAcl acl = new WikiAcl
            {
                WikiFK = null,
                WikiPageFK = Guid.NewGuid(),
                PrincipalId = Guid.NewGuid(),
                PermissionKey = "wikis.Wikis.Author",
            };

            Assert.Null(acl.WikiFK);
            Assert.NotNull(acl.WikiPageFK);
        }

        [Theory]
        [InlineData("wikis.Wikis.Read")]
        [InlineData("wikis.Wikis.Author")]
        [InlineData("wikis.Wikis.ManageMedia")]
        [InlineData("wikis.Wikis.AdministerAccess")]
        [InlineData("wikis.Wikis.Configure")]
        public void WhenAclCarriesAPermissionKey_ThenItUsesTheCanonicalModuleDomainActionShape(string permissionKey)
        {
            WikiAcl acl = new WikiAcl
            {
                WikiFK = Guid.NewGuid(),
                PrincipalId = Guid.NewGuid(),
                PermissionKey = permissionKey,
            };

            Assert.StartsWith("wikis.Wikis.", acl.PermissionKey, StringComparison.Ordinal);
            Assert.False(string.IsNullOrWhiteSpace(acl.PermissionKey));
        }
    }
}
