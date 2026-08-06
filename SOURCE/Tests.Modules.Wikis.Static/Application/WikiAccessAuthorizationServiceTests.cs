using App.Modules.Sys.Application.Domains.Users.Context.Services;
using App.Modules.Sys.Infrastructure.Domains.Configuration.Configuration;
using App.Modules.Sys.Infrastructure.Domains.Configuration.Configuration.Services;
using App.Modules.Sys.Shared.Domains.AccessControl.Models.Enums;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Wikis.Domain.Domains.Wikis.Permissions;
using NSubstitute;
using Tests.Modules.Wikis.Static.Helpers;

namespace Tests.Modules.Wikis.Static.Application
{
    /// <summary>
    /// Tests the wiki ACL resolver's compatibility fallback and explicit-grant behavior.
    /// </summary>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.Authorization)]
    [Trait(TestTraits.Quality, TestTraits.Iso25010.FunctionalSuitability.Correctness)]
    public class WikiAccessAuthorizationServiceTests
    {
        [Fact]
        public async Task WhenNoAclRowsExist_AndCallerIsAuthenticated_ThenPageIsPermitted()
        {
            Guid pageId = Guid.NewGuid();
            Guid wikiId = Guid.NewGuid();

            IWikiAclRepository aclRepository = Substitute.For<IWikiAclRepository>();
            IWikiPageRepository pageRepository = Substitute.For<IWikiPageRepository>();
            IUserContextService userContextService = Substitute.For<IUserContextService>();
            IPrincipalContextFactory principalContextFactory = Substitute.For<IPrincipalContextFactory>();
            IAppLogger logger = Substitute.For<IAppLogger>();

            pageRepository.Query().Returns(new[]
            {
                new WikiPage
                {
                    Id = pageId,
                    WikiFK = wikiId,
                    Path = "dev/notes",
                    Slug = "notes",
                    Title = "Notes",
                    Enabled = true,
                },
            }.AsAsyncQueryable());

            aclRepository.Query().Returns(Array.Empty<WikiAcl>().AsAsyncQueryable());
            userContextService.IsAuthenticated.Returns(true);
            principalContextFactory
                .BuildAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new SettingsPrincipalContext()));

            WikiAccessAuthorizationService service = new WikiAccessAuthorizationService(
                aclRepository,
                pageRepository,
                userContextService,
                principalContextFactory,
                logger);

            bool permitted = await service.IsPagePermittedAsync(
                pageId,
                WikiPermissionsConfigurationObject.Permissions.Author,
                CancellationToken.None);

            Assert.True(permitted);
        }

        [Fact]
        public async Task WhenNoAclRowsExist_AndCallerIsAnonymous_ThenPageIsDenied()
        {
            Guid pageId = Guid.NewGuid();
            Guid wikiId = Guid.NewGuid();

            IWikiAclRepository aclRepository = Substitute.For<IWikiAclRepository>();
            IWikiPageRepository pageRepository = Substitute.For<IWikiPageRepository>();
            IUserContextService userContextService = Substitute.For<IUserContextService>();
            IPrincipalContextFactory principalContextFactory = Substitute.For<IPrincipalContextFactory>();
            IAppLogger logger = Substitute.For<IAppLogger>();

            pageRepository.Query().Returns(new[]
            {
                new WikiPage
                {
                    Id = pageId,
                    WikiFK = wikiId,
                    Path = "dev/notes",
                    Slug = "notes",
                    Title = "Notes",
                    Enabled = true,
                },
            }.AsAsyncQueryable());

            aclRepository.Query().Returns(Array.Empty<WikiAcl>().AsAsyncQueryable());
            userContextService.IsAuthenticated.Returns(false);
            principalContextFactory
                .BuildAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new SettingsPrincipalContext()));

            WikiAccessAuthorizationService service = new WikiAccessAuthorizationService(
                aclRepository,
                pageRepository,
                userContextService,
                principalContextFactory,
                logger);

            bool permitted = await service.IsPagePermittedAsync(
                pageId,
                WikiPermissionsConfigurationObject.Permissions.Author,
                CancellationToken.None);

            Assert.False(permitted);
        }

        [Fact]
        public async Task WhenExplicitGrantExists_ThenMatchingPrincipalIsRequired()
        {
            Guid pageId = Guid.NewGuid();
            Guid wikiId = Guid.NewGuid();
            Guid grantedUserId = Guid.NewGuid();

            IWikiAclRepository aclRepository = Substitute.For<IWikiAclRepository>();
            IWikiPageRepository pageRepository = Substitute.For<IWikiPageRepository>();
            IUserContextService userContextService = Substitute.For<IUserContextService>();
            IPrincipalContextFactory principalContextFactory = Substitute.For<IPrincipalContextFactory>();
            IAppLogger logger = Substitute.For<IAppLogger>();

            pageRepository.Query().Returns(new[]
            {
                new WikiPage
                {
                    Id = pageId,
                    WikiFK = wikiId,
                    Path = "dev/notes",
                    Slug = "notes",
                    Title = "Notes",
                    Enabled = true,
                },
            }.AsAsyncQueryable());

            aclRepository.Query().Returns(new[]
            {
                new WikiAcl
                {
                    Id = Guid.NewGuid(),
                    WikiFK = wikiId,
                    PrincipalId = grantedUserId,
                    PrincipalType = (int)PrincipalType.User,
                    PermissionKey = WikiPermissionsConfigurationObject.Permissions.Author,
                },
            }.AsAsyncQueryable());

            userContextService.IsAuthenticated.Returns(true);
            principalContextFactory
                .BuildAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new SettingsPrincipalContext
                {
                    UserId = Guid.NewGuid(),
                }));

            WikiAccessAuthorizationService service = new WikiAccessAuthorizationService(
                aclRepository,
                pageRepository,
                userContextService,
                principalContextFactory,
                logger);

            bool permitted = await service.IsPagePermittedAsync(
                pageId,
                WikiPermissionsConfigurationObject.Permissions.Author,
                CancellationToken.None);

            Assert.False(permitted);
        }
    }
}
