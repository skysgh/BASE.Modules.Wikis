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
using NSubstitute;
using Tests.Modules.Wikis.Static.Helpers;

namespace Tests.Modules.Wikis.Static.Application
{
    /// <summary>
    /// Phase-A gate (A4): the wiki media <b>byte-level</b> object-store round
    /// trip. <see cref="WikiMediaApplicationService"/> must push the actual
    /// media bytes through the Sys media pipeline on store, and pull them back
    /// from the object store on retrieve, addressing the blob by a path
    /// deterministically derived from the immutable <c>BlobId</c>
    /// (<see cref="WikiMediaBlobPathFactory"/>).
    /// <para>
    /// This replaces the earlier deliberate failing prompt: the binding is now
    /// implemented, so the gate is expressed as a real upload → store → retrieve
    /// assertion against the mocked Sys seams.
    /// </para>
    /// </summary>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.Media)]
    [Trait(TestTraits.Quality, TestTraits.Iso25010.FunctionalSuitability.Correctness)]
    public class WikiMediaByteTransferGateTests
    {
        [Fact]
        public async Task WhenMediaBytesAreStored_ThenTheyArePushedThroughTheMediaPipelineAndHandleReflectsStorageResult()
        {
            // Arrange ----------------------------------------------------------
            Guid pageId = Guid.NewGuid();
            byte[] payload = Encoding.UTF8.GetBytes("the quick brown fox");
            const string mediaType = "image/png";
            const string storedHash = "sha256:deadbeef";

            WikiMediaWriteDto writeDto = new WikiMediaWriteDto
            {
                WikiPageFK = pageId,
                // Caller-supplied identity/integrity must be ignored by the
                // store path; we set bogus values to prove they are replaced.
                BlobId = Guid.NewGuid(),
                ContentHash = "sha256:CALLER-SUPPLIED-IGNORED",
                MediaType = mediaType,
                Title = "Diagram",
                Description = "A diagram",
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();
            IAppLogger logger = Substitute.For<IAppLogger>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IObjectStorageService objectStorageService =
                Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();

            // This gate is about byte transfer, not authorization: permit all.
            accessAuthorizationService
                .IsPagePermittedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            // The Sys media pipeline returns the authoritative storage result.
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
                    ContentSizeBytes: payload.LongLength,
                    ContentHash: storedHash,
                    WidthPx: null,
                    HeightPx: null)));

            // Persist echoes the entity back (assigning an identity).
            repository.CreateAsync(Arg.Any<WikiMedia>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    WikiMedia entity = callInfo.Arg<WikiMedia>();
                    entity.Id = Guid.NewGuid();
                    return Task.FromResult(entity);
                });

            // Map entity -> read DTO (the service maps the persisted entity back).
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

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            using MemoryStream content = new MemoryStream(payload);
            WikiMediaReadDto readDto = await service.StoreMediaAsync(
                writeDto,
                content,
                CancellationToken.None);

            // Assert -----------------------------------------------------------
            // A fresh, non-empty blob id was allocated (NOT the caller's).
            Assert.NotEqual(Guid.Empty, readDto.BlobId);
            Assert.NotEqual(writeDto.BlobId, readDto.BlobId);

            // Identity/integrity came from the storage result, not the DTO.
            Assert.Equal(storedHash, readDto.ContentHash);
            Assert.Equal(mediaType, readDto.MediaType);
            Assert.Equal(pageId, readDto.WikiPageFK);

            // Bytes were pushed through the Sys media pipeline, into the private
            // container, at the deterministically derived path.
            string expectedPath = WikiMediaBlobPathFactory.BuildBlobPath(readDto.BlobId, mediaType);
            await mediaUploadInfrastructureService.Received(1).UploadAsync(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                mediaType,
                StorageContainerType.Private,
                expectedPath,
                Arg.Any<BlobUploadMetadata?>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenMediaBytesAreRetrieved_ThenTheyArePulledFromTheObjectStoreAtTheDerivedPath()
        {
            // Arrange ----------------------------------------------------------
            Guid mediaId = Guid.NewGuid();
            Guid blobId = Guid.NewGuid();
            const string mediaType = "application/pdf";
            byte[] payload = Encoding.UTF8.GetBytes("%PDF-1.7 ...");

            WikiMedia handle = new WikiMedia
            {
                Id = mediaId,
                WikiPageFK = Guid.NewGuid(),
                BlobId = blobId,
                MediaType = mediaType,
                ContentHash = "sha256:abc",
                Title = "Spec",
            };

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();
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

            repository.GetForUpdateAsync(mediaId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(handle));

            string expectedPath = WikiMediaBlobPathFactory.BuildBlobPath(blobId, mediaType);
            objectStorageService
                .DownloadAsync(StorageContainerType.Private, expectedPath, Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult<Stream>(new MemoryStream(payload)));

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            (Stream Content, string MediaType)? result =
                await service.GetMediaBytesAsync(mediaId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.NotNull(result);
            Assert.Equal(mediaType, result!.Value.MediaType);

            using MemoryStream buffer = new MemoryStream();
            await result.Value.Content.CopyToAsync(buffer);
            Assert.Equal(payload, buffer.ToArray());
            await result.Value.Content.DisposeAsync();
        }

        [Fact]
        public async Task WhenMediaHandleDoesNotExist_ThenRetrieveReturnsNull()
        {
            // Arrange ----------------------------------------------------------
            Guid missingId = Guid.NewGuid();

            ICrustStateRepository<WikiMedia> repository =
                Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();
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

            repository.GetForUpdateAsync(missingId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMedia?>(null));

            WikiMediaApplicationService service = new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);

            // Act --------------------------------------------------------------
            (Stream Content, string MediaType)? result =
                await service.GetMediaBytesAsync(missingId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.Null(result);
            await objectStorageService.DidNotReceive().DownloadAsync(
                Arg.Any<StorageContainerType>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }
    }
}
