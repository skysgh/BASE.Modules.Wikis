using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Domains.Media.Services;
using App.Modules.Sys.Shared.ObjectStorage.Services;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using NSubstitute;

namespace Tests.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Phase-A gate tests for <see cref="WikiMediaApplicationService"/>. These
    /// exercise the immutable-blob handle lifecycle: creating a media handle
    /// must preserve the object-store <c>BlobId</c>, the IANA media type and the
    /// content hash unchanged through the map → persist → map-back round trip
    /// (ADR-018: "replace" means a new blob id and a repoint, never a mutation).
    /// </summary>
    public class WikiMediaApplicationServiceTests
    {
        private static WikiMediaApplicationService CreateService(
            ICrustStateRepository<WikiMedia> repository,
            IObjectMappingService mapper)
        {
            IAppLogger logger = Substitute.For<IAppLogger>();
            IMediaUploadInfrastructureService mediaUploadInfrastructureService =
                Substitute.For<IMediaUploadInfrastructureService>();
            IObjectStorageService objectStorageService = Substitute.For<IObjectStorageService>();
            IWikiAccessAuthorizationService accessAuthorizationService =
                Substitute.For<IWikiAccessAuthorizationService>();
            accessAuthorizationService
                .IsPagePermittedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));
            return new WikiMediaApplicationService(
                repository,
                mapper,
                logger,
                mediaUploadInfrastructureService,
                objectStorageService,
                accessAuthorizationService);
        }

        [Fact]
        public async Task WhenMediaHandleIsCreated_ThenBlobIdMediaTypeAndHashAreRoundTrippedUnchanged()
        {
            Guid blobId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();
            WikiMediaWriteDto writeDto = new WikiMediaWriteDto
            {
                WikiPageFK = pageId,
                BlobId = blobId,
                MediaType = "image/svg+xml",
                ContentHash = "sha256:abc123",
                Title = "Architecture diagram",
            };

            ICrustStateRepository<WikiMedia> repository = Substitute.For<ICrustStateRepository<WikiMedia>>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();

            // Map write DTO -> entity (the service maps before persisting).
            mapper.Map<WikiMediaWriteDto, WikiMedia>(Arg.Any<WikiMediaWriteDto>())
                .Returns(callInfo =>
                {
                    WikiMediaWriteDto source = callInfo.Arg<WikiMediaWriteDto>();
                    return new WikiMedia
                    {
                        WikiPageFK = source.WikiPageFK,
                        BlobId = source.BlobId,
                        MediaType = source.MediaType,
                        ContentHash = source.ContentHash,
                        Title = source.Title,
                        Description = source.Description,
                    };
                });

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

            WikiMediaApplicationService service = CreateService(repository, mapper);

            WikiMediaReadDto readDto = await service.CreateAsync(writeDto, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, readDto.Id);
            Assert.Equal(pageId, readDto.WikiPageFK);
            Assert.Equal(blobId, readDto.BlobId);
            Assert.Equal("image/svg+xml", readDto.MediaType);
            Assert.Equal("sha256:abc123", readDto.ContentHash);

            await repository.Received(1).CreateAsync(
                Arg.Is<WikiMedia>(media => media.BlobId == blobId && media.ContentHash == "sha256:abc123"),
                Arg.Any<CancellationToken>());
        }
    }
}
