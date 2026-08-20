using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Infrastructure.Services.Contracts;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Configuration.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Permissions;
using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using NSubstitute;
using Tests.Modules.Wikis.Static.Helpers;

namespace Tests.Modules.Wikis.Static.Application
{
    /// <summary>
    /// Gate tests proving page-content saves use the same share-based authoring
    /// permission model as wiki-media writes.
    /// </summary>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.Authorization)]
    [Trait(TestTraits.Quality, TestTraits.Iso25010.FunctionalSuitability.Correctness)]
    public class WikiPageAuthorizationGateTests
    {
        [Fact]
        public async Task WhenCreatingPage_AndCallerLacksWikiAuthorGrant_ThenSaveIsDeniedBeforePageIsCreated()
        {
            ICrustStateRepository<WikiPage> pageRepository = Substitute.For<ICrustStateRepository<WikiPage>>();
            IWikiRepository wikiRepository = Substitute.For<IWikiRepository>();
            IWikiPageVersionRepository versionRepository = Substitute.For<IWikiPageVersionRepository>();
            IWikiBodyStoreCoordinator bodyStoreCoordinator = Substitute.For<IWikiBodyStoreCoordinator>();
            IAppConfiguration<WikiConfigurationObject> configuration = Substitute.For<IAppConfiguration<WikiConfigurationObject>>();
            IWikiAccessAuthorizationService accessAuthorizationService = Substitute.For<IWikiAccessAuthorizationService>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();
            IAppLogger logger = Substitute.For<IAppLogger>();

            Guid wikiId = Guid.NewGuid();
            pageRepository.Query().Returns(Array.Empty<WikiPage>().AsAsyncQueryable());
            accessAuthorizationService
                .IsWikiPermittedAsync(
                    wikiId,
                    WikiPermissionsConfigurationObject.Permissions.Author,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));

            WikiPageApplicationService service = new WikiPageApplicationService(
                pageRepository,
                wikiRepository,
                versionRepository,
                bodyStoreCoordinator,
                configuration,
                accessAuthorizationService,
                mapper,
                logger);

            WikiPageContentWriteDto request = new WikiPageContentWriteDto
            {
                WikiFK = wikiId,
                Path = "home",
                Title = "Home",
                Body = "Body",
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.SaveContentAsync(request, CancellationToken.None));

            await pageRepository.DidNotReceive().CreateAsync(Arg.Any<WikiPage>(), Arg.Any<CancellationToken>());
            await bodyStoreCoordinator.DidNotReceive().StoreBodyAsync(
                Arg.Any<WikiBodyStoreContext>(),
                Arg.Any<ReadOnlyMemory<byte>>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenUpdatingPage_AndCallerLacksPageAuthorGrant_ThenSaveIsDeniedBeforeVersioningRuns()
        {
            ICrustStateRepository<WikiPage> pageRepository = Substitute.For<ICrustStateRepository<WikiPage>>();
            IWikiRepository wikiRepository = Substitute.For<IWikiRepository>();
            IWikiPageVersionRepository versionRepository = Substitute.For<IWikiPageVersionRepository>();
            IWikiBodyStoreCoordinator bodyStoreCoordinator = Substitute.For<IWikiBodyStoreCoordinator>();
            IAppConfiguration<WikiConfigurationObject> configuration = Substitute.For<IAppConfiguration<WikiConfigurationObject>>();
            IWikiAccessAuthorizationService accessAuthorizationService = Substitute.For<IWikiAccessAuthorizationService>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();
            IAppLogger logger = Substitute.For<IAppLogger>();

            Guid wikiId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();
            WikiPage existingPage = new WikiPage
            {
                Id = pageId,
                WikiFK = wikiId,
                Path = "home",
                Slug = "home",
                Title = "Home",
                Enabled = true,
            };

            pageRepository.Query().Returns(new[] { existingPage }.AsAsyncQueryable());
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    WikiPermissionsConfigurationObject.Permissions.Author,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));

            WikiPageApplicationService service = new WikiPageApplicationService(
                pageRepository,
                wikiRepository,
                versionRepository,
                bodyStoreCoordinator,
                configuration,
                accessAuthorizationService,
                mapper,
                logger);

            WikiPageContentWriteDto request = new WikiPageContentWriteDto
            {
                WikiFK = wikiId,
                Path = "home",
                Title = "Home",
                Body = "Body",
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.SaveContentAsync(request, CancellationToken.None));

            await pageRepository.DidNotReceive().GetForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
            await versionRepository.DidNotReceive().CreateAsync(Arg.Any<WikiPageVersion>(), Arg.Any<CancellationToken>());
            await bodyStoreCoordinator.DidNotReceive().StoreBodyAsync(
                Arg.Any<WikiBodyStoreContext>(),
                Arg.Any<ReadOnlyMemory<byte>>(),
                Arg.Any<CancellationToken>());
        }
    }
}
