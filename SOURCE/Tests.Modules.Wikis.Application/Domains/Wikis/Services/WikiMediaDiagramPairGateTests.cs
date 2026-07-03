using System.Text;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Domains.Media;
using App.Modules.Sys.Shared.Domains.Media.Services;
using App.Modules.Sys.Shared.ObjectStorage.Models;
using App.Modules.Sys.Shared.ObjectStorage.Models.Enums;
using App.Modules.Sys.Shared.ObjectStorage.Services;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using NSubstitute;

namespace Tests.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Phase-A gate for the draw.io two-artifact pairing (ADR-018). A diagram is
    /// stored as an editable <b>source</b> (mxfile) plus a display <b>render</b>
    /// (SVG); the render points back to the source via
    /// <see cref="WikiMediaWriteDto.SourceMediaFK"/> so the editor can reopen the
    /// source for edit. These tests exercise
    /// <see cref="WikiMediaApplicationService.StoreDiagramAsync"/> and
    /// <see cref="WikiMediaApplicationService.GetDiagramSourceAsync"/> against the
    /// mocked Sys seams: the pair must be stored under one owning page with the
    /// two fixed media types, behind a single share-based authorization check,
    /// and the source must be resolvable from the render.
    /// </summary>
    public class WikiMediaDiagramPairGateTests
    {
        private static IObjectMappingService CreateEchoMapper()
        {
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();

            // Map entity -> read DTO, preserving the render -> source link so the
            // pairing is observable on the returned DTOs.
            mapper.Map<WikiMedia, WikiMediaReadDto>(Arg.Any<WikiMedia>())
                .Returns(callInfo =>
                {
                    WikiMedia entity = callInfo.Arg<WikiMedia>();
                    return new WikiMediaReadDto
                    {
                        Id = entity.Id,
                        WikiPageFK = entity.WikiPageFK,
                        BlobId = entity.BlobId,
                        MediaType = entity.MediaType,
                        ContentHash = entity.ContentHash,
                        SourceMediaFK = entity.SourceMediaFK,
                        Title = entity.Title,
                        Description = entity.Description,
                    };
                });

            return mapper;
        }

        private static void StubUploadEchoesPath(
            IMediaUploadInfrastructureService mediaUploadInfrastructureService,
            string contentHash)
        {
            mediaUploadInfrastructureService
                .UploadAsync(
                    Arg.Any<Stream>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<StorageContainerType>(),
                    Arg.Any<string>(),
                    Arg.Any<BlobUploadMetadata?>(),
                    Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(new MediaUploadResult(
                    BlobPath: callInfo.ArgAt<string>(4),
                    MediaType: callInfo.ArgAt<string>(2),
                    ContentSizeBytes: 1,
                    ContentHash: contentHash,
                    WidthPx: null,
                    HeightPx: null)));
        }

        [Fact]
        public async Task WhenDiagramIsStored_ThenSourceAndRenderArePersistedWithFixedTypesAndRenderLinksToSource()
        {
            // Arrange ----------------------------------------------------------
            Guid pageId = Guid.NewGuid();
            byte[] sourceBytes = Encoding.UTF8.GetBytes("<mxfile>...</mxfile>");
            byte[] renderBytes = Encoding.UTF8.GetBytes("<svg>...</svg>");

            WikiMediaWriteDto metadata = new WikiMediaWriteDto
            {
                WikiPageFK = pageId,
                // Any media type on the DTO must be ignored for the diagram pair.
                MediaType = "text/plain",
                Title = "Architecture",
                Description = "System overview",
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = CreateEchoMapper();
            IAppLogger logger = Substitute.For<IAppLogger>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();
            accessAuthorizationService
                .IsPagePermittedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            StubUploadEchoesPath(mediaUploadInfrastructureService, "sha256:stored");

            // Persist echoes the entity back (assigning an identity).
            repository.CreateAsync(Arg.Any<WikiMedia>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    WikiMedia entity = callInfo.Arg<WikiMedia>();
                    entity.Id = Guid.NewGuid();
                    return Task.FromResult(entity);
                });

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            using MemoryStream source = new MemoryStream(sourceBytes);
            using MemoryStream render = new MemoryStream(renderBytes);
            (WikiMediaReadDto Render, WikiMediaReadDto Source) result =
                await service.StoreDiagramAsync(metadata, source, render, CancellationToken.None);

            // Assert -----------------------------------------------------------
            // Both artifacts belong to the owning page.
            Assert.Equal(pageId, result.Source.WikiPageFK);
            Assert.Equal(pageId, result.Render.WikiPageFK);

            // The fixed diagram media types were used, not the DTO's.
            Assert.Equal(WikiDomainConstants.DrawioSourceMediaType, result.Source.MediaType);
            Assert.Equal(WikiDomainConstants.DrawioRenderMediaType, result.Render.MediaType);

            // The source has no link; the render points back at the source.
            Assert.Null(result.Source.SourceMediaFK);
            Assert.Equal(result.Source.Id, result.Render.SourceMediaFK);

            // Exactly two blobs were stored, one per fixed media type.
            await mediaUploadInfrastructureService.Received(2).UploadAsync(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                StorageContainerType.Private,
                Arg.Any<string>(),
                Arg.Any<BlobUploadMetadata?>(),
                Arg.Any<CancellationToken>());

            // A single share-based authorization check governs the whole pair.
            await accessAuthorizationService.Received(1).IsPagePermittedAsync(
                pageId,
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenStoringDiagram_AndCallerIsNotPermitted_ThenNothingIsStored()
        {
            // Arrange ----------------------------------------------------------
            Guid pageId = Guid.NewGuid();

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = CreateEchoMapper();
            IAppLogger logger = Substitute.For<IAppLogger>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();

            // Deny both media-management and content-authoring so the share-based
            // gate short-circuits before any storage.
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    App.Modules.Wikis.Domain.Domains.Wikis.Permissions.WikiPermissionsConfigurationObject.Permissions.ManageMedia,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    App.Modules.Wikis.Domain.Domains.Wikis.Permissions.WikiPermissionsConfigurationObject.Permissions.Author,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            WikiMediaWriteDto metadata = new WikiMediaWriteDto { WikiPageFK = pageId };

            // Act / Assert -----------------------------------------------------
            using MemoryStream source = new MemoryStream(Encoding.UTF8.GetBytes("<mxfile/>"));
            using MemoryStream render = new MemoryStream(Encoding.UTF8.GetBytes("<svg/>"));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.StoreDiagramAsync(metadata, source, render, CancellationToken.None));

            // No bytes stored and no handle created on denial.
            await mediaUploadInfrastructureService.DidNotReceive().UploadAsync(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<StorageContainerType>(),
                Arg.Any<string>(),
                Arg.Any<BlobUploadMetadata?>(),
                Arg.Any<CancellationToken>());
            await repository.DidNotReceive().CreateAsync(
                Arg.Any<WikiMedia>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenStoringDiagram_AndCallerCanAuthor_ThenPairIsStoredWithoutManageMediaGrant()
        {
            // Arrange ----------------------------------------------------------
            Guid pageId = Guid.NewGuid();

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = CreateEchoMapper();
            IAppLogger logger = Substitute.For<IAppLogger>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();

            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    App.Modules.Wikis.Domain.Domains.Wikis.Permissions.WikiPermissionsConfigurationObject.Permissions.ManageMedia,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    App.Modules.Wikis.Domain.Domains.Wikis.Permissions.WikiPermissionsConfigurationObject.Permissions.Author,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            StubUploadEchoesPath(mediaUploadInfrastructureService, "sha256:stored");

            repository.CreateAsync(Arg.Any<WikiMedia>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    WikiMedia entity = callInfo.Arg<WikiMedia>();
                    entity.Id = Guid.NewGuid();
                    return Task.FromResult(entity);
                });

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            WikiMediaWriteDto metadata = new WikiMediaWriteDto
            {
                WikiPageFK = pageId,
                Title = "Architecture",
            };

            // Act --------------------------------------------------------------
            using MemoryStream source = new MemoryStream(Encoding.UTF8.GetBytes("<mxfile/>"));
            using MemoryStream render = new MemoryStream(Encoding.UTF8.GetBytes("<svg/>"));
            (WikiMediaReadDto Render, WikiMediaReadDto Source) result =
                await service.StoreDiagramAsync(metadata, source, render, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.Equal(pageId, result.Render.WikiPageFK);
            Assert.Equal(pageId, result.Source.WikiPageFK);
            await mediaUploadInfrastructureService.Received(2).UploadAsync(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                StorageContainerType.Private,
                Arg.Any<string>(),
                Arg.Any<BlobUploadMetadata?>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenDiagramSourceIsRequested_ThenTheLinkedSourceHandleIsReturned()
        {
            // Arrange ----------------------------------------------------------
            Guid renderId = Guid.NewGuid();
            Guid sourceId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();

            WikiMedia renderHandle = new WikiMedia
            {
                Id = renderId,
                WikiPageFK = pageId,
                BlobId = Guid.NewGuid(),
                MediaType = WikiDomainConstants.DrawioRenderMediaType,
                ContentHash = "sha256:render",
                SourceMediaFK = sourceId,
                Title = "Architecture",
            };

            WikiMedia sourceHandle = new WikiMedia
            {
                Id = sourceId,
                WikiPageFK = pageId,
                BlobId = Guid.NewGuid(),
                MediaType = WikiDomainConstants.DrawioSourceMediaType,
                ContentHash = "sha256:source",
                Title = "Architecture",
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = CreateEchoMapper();
            IAppLogger logger = Substitute.For<IAppLogger>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();
            accessAuthorizationService
                .IsPagePermittedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            repository.GetForUpdateAsync(renderId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(renderHandle));
            repository.GetForUpdateAsync(sourceId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(sourceHandle));

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            WikiMediaReadDto? source =
                await service.GetDiagramSourceAsync(renderId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.NotNull(source);
            Assert.Equal(sourceId, source!.Id);
            Assert.Equal(WikiDomainConstants.DrawioSourceMediaType, source.MediaType);
        }

        [Fact]
        public async Task WhenRenderHasNoLinkedSource_ThenDiagramSourceReturnsNull()
        {
            // Arrange ----------------------------------------------------------
            Guid renderId = Guid.NewGuid();

            WikiMedia plainHandle = new WikiMedia
            {
                Id = renderId,
                WikiPageFK = Guid.NewGuid(),
                BlobId = Guid.NewGuid(),
                MediaType = "image/png",
                ContentHash = "sha256:plain",
                // No SourceMediaFK: this is a plain media handle, not a diagram.
                Title = "Photo",
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = CreateEchoMapper();
            IAppLogger logger = Substitute.For<IAppLogger>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();
            accessAuthorizationService
                .IsPagePermittedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            repository.GetForUpdateAsync(renderId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(plainHandle));

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            WikiMediaReadDto? source =
                await service.GetDiagramSourceAsync(renderId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.Null(source);
        }
    }
}
