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
using App.Modules.Wikis.Domain.Domains.Wikis;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Permissions;
using NSubstitute;
using Tests.Modules.Wikis.Static.Helpers;

namespace Tests.Modules.Wikis.Static.Application
{
    /// <summary>
    /// Gate tests proving the wiki-media round trip is secured strictly in the
    /// Application layer via the share-based <see cref="WikiAcl"/> model (no
    /// transport-level <c>[Authorize]</c>), and that an authorized retrieval is
    /// delivered according to the configured
    /// <see cref="MediaDeliveryMode"/> (Proxy stream vs Direct signed URL).
    /// </summary>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.Media)]
    [Trait(TestTraits.Quality, TestTraits.Iso25010.FunctionalSuitability.Correctness)]
    public class WikiMediaAccessGateTests
    {
        private static WikiMediaApplicationService CreateService(
            ICrustStateRepository<WikiMedia> repository,
            IObjectStorageService objectStorageService,
            IWikiAccessAuthorizationService accessAuthorizationService,
            IMediaUploadInfrastructureService? mediaUploadInfrastructureService = null,
            IObjectMappingService? mapper = null)
        {
            IAppLogger logger = Substitute.For<IAppLogger>();
            mediaUploadInfrastructureService ??= Substitute.For<IMediaUploadInfrastructureService>();
            mapper ??= Substitute.For<IObjectMappingService>();
            return new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);
        }

        [Fact]
        public async Task WhenStoringMedia_AndCallerLacksManageMediaGrant_ThenUploadIsDeniedAndNoBytesAreStored()
        {
            // Arrange ----------------------------------------------------------
            Guid pageId = Guid.NewGuid();
            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();

            // Deny both media-management and content-authoring for this caller.
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    WikiPermissionsConfigurationObject.Permissions.ManageMedia,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    WikiPermissionsConfigurationObject.Permissions.Author,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));

            WikiMediaApplicationService service = CreateService(
                repository,
                objectStorageService,
                accessAuthorizationService,
                mediaUploadInfrastructureService);

            WikiMediaWriteDto writeDto = new WikiMediaWriteDto
            {
                WikiPageFK = pageId,
                MediaType = "image/png",
                Title = "Diagram",
            };

            // Act + Assert -----------------------------------------------------
            using MemoryStream content = new MemoryStream(Encoding.UTF8.GetBytes("bytes"));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.StoreMediaAsync(writeDto, content, CancellationToken.None));

            // The bytes must never reach the media pipeline when denied.
            await mediaUploadInfrastructureService.DidNotReceive().UploadAsync(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<StorageContainerType>(),
                Arg.Any<string>(),
                Arg.Any<BlobUploadMetadata?>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenStoringMedia_AndCallerCanAuthor_ThenUploadIsAllowedWithoutManageMediaGrant()
        {
            // Arrange ----------------------------------------------------------
            Guid pageId = Guid.NewGuid();
            Guid mediaId = Guid.NewGuid();
            Guid blobId = Guid.NewGuid();

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();

            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    WikiPermissionsConfigurationObject.Permissions.ManageMedia,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    WikiPermissionsConfigurationObject.Permissions.Author,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            mediaUploadInfrastructureService
                .UploadAsync(
                    Arg.Any<Stream>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<StorageContainerType>(),
                    Arg.Any<string>(),
                    Arg.Any<BlobUploadMetadata?>(),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new MediaUploadResult(
                    BlobPath: "wiki-media/path",
                    MediaType: "image/png",
                    ContentSizeBytes: 5,
                    ContentHash: "sha256:test",
                    WidthPx: null,
                    HeightPx: null)));

            repository.CreateAsync(Arg.Any<WikiMedia>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    WikiMedia entity = callInfo.Arg<WikiMedia>();
                    entity.Id = mediaId;
                    entity.BlobId = blobId;
                    return Task.FromResult(entity);
                });

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
                        Title = entity.Title,
                        Description = entity.Description,
                    };
                });

            WikiMediaApplicationService service = CreateService(
                repository,
                objectStorageService,
                accessAuthorizationService,
                mediaUploadInfrastructureService,
                mapper);

            WikiMediaWriteDto writeDto = new WikiMediaWriteDto
            {
                WikiPageFK = pageId,
                MediaType = "image/png",
                Title = "Diagram",
            };

            // Act --------------------------------------------------------------
            using MemoryStream content = new MemoryStream(Encoding.UTF8.GetBytes("bytes"));
            WikiMediaReadDto result = await service.StoreMediaAsync(writeDto, content, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.Equal(pageId, result.WikiPageFK);
            Assert.Equal("image/png", result.MediaType);
            await mediaUploadInfrastructureService.Received(1).UploadAsync(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                StorageContainerType.Private,
                Arg.Any<string>(),
                Arg.Any<BlobUploadMetadata?>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenRetrievingMedia_AndCallerLacksReadGrant_ThenResultIsNullAndStoreIsNeverTouched()
        {
            // Arrange ----------------------------------------------------------
            Guid mediaId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();

            WikiMedia handle = new WikiMedia
            {
                Id = mediaId,
                WikiPageFK = pageId,
                BlobId = Guid.NewGuid(),
                MediaType = "image/png",
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();

            repository.GetForUpdateAsync(mediaId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(handle));

            // Deny the Read permission for this caller.
            accessAuthorizationService
                .IsPagePermittedAsync(
                    pageId,
                    WikiPermissionsConfigurationObject.Permissions.Read,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));

            WikiMediaApplicationService service = CreateService(
                repository,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            (Stream Content, string MediaType)? bytesResult =
                await service.GetMediaBytesAsync(mediaId, CancellationToken.None);
            WikiMediaRetrievalResult? deliveryResult =
                await service.GetMediaForDeliveryAsync(mediaId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            // Denied and missing are indistinguishable: both collapse to null so
            // the service never acts as an existence/content oracle.
            Assert.Null(bytesResult);
            Assert.Null(deliveryResult);

            await objectStorageService.DidNotReceive().DownloadAsync(
                Arg.Any<StorageContainerType>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
            await objectStorageService.DidNotReceive().GenerateSignedReadUrlAsync(
                Arg.Any<string>(),
                Arg.Any<TimeSpan?>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenRetrievingForDelivery_AndModeIsProxy_ThenBytesAreStreamedThroughTheBackend()
        {
            // Arrange ----------------------------------------------------------
            Guid mediaId = Guid.NewGuid();
            Guid blobId = Guid.NewGuid();
            const string mediaType = "image/png";
            byte[] payload = Encoding.UTF8.GetBytes("PNGDATA");

            WikiMedia handle = new WikiMedia
            {
                Id = mediaId,
                WikiPageFK = Guid.NewGuid(),
                BlobId = blobId,
                MediaType = mediaType,
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();

            repository.GetForUpdateAsync(mediaId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(handle));
            accessAuthorizationService
                .IsPagePermittedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            objectStorageService.MediaDeliveryMode.Returns(MediaDeliveryMode.Proxy);
            string expectedPath = WikiMediaBlobPathFactory.BuildBlobPath(blobId, mediaType);
            objectStorageService
                .DownloadAsync(StorageContainerType.Private, expectedPath, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Stream>(new MemoryStream(payload)));

            WikiMediaApplicationService service = CreateService(
                repository,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            WikiMediaRetrievalResult? result =
                await service.GetMediaForDeliveryAsync(mediaId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.NotNull(result);
            Assert.Equal(MediaDeliveryMode.Proxy, result!.Mode);
            Assert.Equal(mediaType, result.MediaType);
            Assert.NotNull(result.Content);
            Assert.Null(result.SignedUrl);

            await objectStorageService.DidNotReceive().GenerateSignedReadUrlAsync(
                Arg.Any<string>(),
                Arg.Any<TimeSpan?>(),
                Arg.Any<CancellationToken>());

            await result.Content!.DisposeAsync();
        }

        [Fact]
        public async Task WhenRetrievingForDelivery_AndModeIsDirect_ThenASignedUrlIsReturnedAndNoBytesAreStreamed()
        {
            // Arrange ----------------------------------------------------------
            Guid mediaId = Guid.NewGuid();
            Guid blobId = Guid.NewGuid();
            const string mediaType = "application/pdf";
            const string signedUrl = "https://storage.example/media-signed/abc?sig=xyz";

            WikiMedia handle = new WikiMedia
            {
                Id = mediaId,
                WikiPageFK = Guid.NewGuid(),
                BlobId = blobId,
                MediaType = mediaType,
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();

            repository.GetForUpdateAsync(mediaId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(handle));
            accessAuthorizationService
                .IsPagePermittedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            objectStorageService.MediaDeliveryMode.Returns(MediaDeliveryMode.Direct);
            string expectedPath = WikiMediaBlobPathFactory.BuildBlobPath(blobId, mediaType);
            objectStorageService
                .GenerateSignedReadUrlAsync(expectedPath, Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new SignedUrlResult
                {
                    Url = signedUrl,
                    BlobPath = expectedPath,
                    ContainerType = StorageContainerType.Private,
                    ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(15),
                    ValidFor = TimeSpan.FromSeconds(15),
                }));

            WikiMediaApplicationService service = CreateService(
                repository,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            WikiMediaRetrievalResult? result =
                await service.GetMediaForDeliveryAsync(mediaId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.NotNull(result);
            Assert.Equal(MediaDeliveryMode.Direct, result!.Mode);
            Assert.Equal(mediaType, result.MediaType);
            Assert.Equal(signedUrl, result.SignedUrl);
            Assert.Null(result.Content);

            // Direct mode must not stream bytes through the backend.
            await objectStorageService.DidNotReceive().DownloadAsync(
                Arg.Any<StorageContainerType>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }
    }
}
