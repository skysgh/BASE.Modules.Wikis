using System.Text;
using App.Modules.Sys.Shared.ObjectStorage.Models.Enums;
using App.Modules.Wikis.Application.Domains.Wikis.Services;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Tests.Modules.Wikis.Interfaces.API.REST.Domains.V1.Wikis
{
    /// <summary>
    /// Phase-D step 16 gate: the wiki media controller's <b>byte</b> round trip.
    /// The CRUST surface addresses the immutable media <i>handle</i>; these two
    /// actions (<c>POST/GET {id}/bytes</c>) address the underlying object-store
    /// <i>bytes</i>. The controller must delegate faithfully to
    /// <see cref="IWikiMediaApplicationService"/> without owning identity,
    /// integrity, or storage-path concerns itself.
    /// </summary>
    public class WikiMediaControllerByteEndpointGateTests
    {
        [Fact]
        public async Task WhenBytesAreUploaded_ThenTheControllerStoresThemAndReturnsCreatedHandle()
        {
            // Arrange ----------------------------------------------------------
            Guid pageId = Guid.NewGuid();
            byte[] payload = Encoding.UTF8.GetBytes("the quick brown fox");
            const string mediaType = "image/png";
            const string title = "Diagram";
            const string description = "A diagram";

            IWikiMediaApplicationService service =
                Substitute.For<IWikiMediaApplicationService>();

            WikiMediaReadDto storedHandle = new WikiMediaReadDto
            {
                Id = Guid.NewGuid(),
                WikiPageFK = pageId,
                BlobId = Guid.NewGuid(),
                MediaType = mediaType,
                ContentHash = "sha256:deadbeef",
                Title = title,
                Description = description,
            };

            service
                .StoreMediaAsync(Arg.Any<WikiMediaWriteDto>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(storedHandle));

            WikiMediaController controller = new WikiMediaController(service);
            IFormFile file = BuildFormFile(payload, mediaType, "diagram.png");

            // Act --------------------------------------------------------------
            ActionResult<WikiMediaReadDto> result = await controller.UploadBytesAsync(
                Guid.NewGuid(),
                pageId,
                file,
                title,
                description,
                CancellationToken.None);

            // Assert -----------------------------------------------------------
            CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result.Result);
            WikiMediaReadDto returned = Assert.IsType<WikiMediaReadDto>(created.Value);
            Assert.Equal(storedHandle.Id, returned.Id);

            // The controller forwarded the descriptive metadata; identity and
            // integrity are owned by the store, not asserted here.
            await service.Received(1).StoreMediaAsync(
                Arg.Is<WikiMediaWriteDto>(dto =>
                    dto.WikiPageFK == pageId &&
                    dto.MediaType == mediaType &&
                    dto.Title == title &&
                    dto.Description == description),
                Arg.Any<Stream>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenNoFileIsSupplied_ThenUploadReturnsBadRequest()
        {
            // Arrange ----------------------------------------------------------
            IWikiMediaApplicationService service =
                Substitute.For<IWikiMediaApplicationService>();
            WikiMediaController controller = new WikiMediaController(service);
            IFormFile emptyFile = BuildFormFile(Array.Empty<byte>(), "image/png", "empty.png");

            // Act --------------------------------------------------------------
            ActionResult<WikiMediaReadDto> result = await controller.UploadBytesAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                emptyFile,
                title: null,
                description: null,
                CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.IsType<BadRequestObjectResult>(result.Result);
            await service.DidNotReceive().StoreMediaAsync(
                Arg.Any<WikiMediaWriteDto>(),
                Arg.Any<Stream>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenBytesAreRetrieved_AndModeIsProxy_ThenTheControllerStreamsThemWithTheStoredMediaType()
        {
            // Arrange ----------------------------------------------------------
            Guid mediaId = Guid.NewGuid();
            const string mediaType = "application/pdf";
            byte[] payload = Encoding.UTF8.GetBytes("%PDF-1.7 ...");

            IWikiMediaApplicationService service =
                Substitute.For<IWikiMediaApplicationService>();

            service
                .GetMediaForDeliveryAsync(mediaId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMediaRetrievalResult?>(new WikiMediaRetrievalResult
                {
                    Mode = MediaDeliveryMode.Proxy,
                    MediaType = mediaType,
                    Content = new MemoryStream(payload),
                }));

            WikiMediaController controller = new WikiMediaController(service);

            // Act --------------------------------------------------------------
            IActionResult result = await controller.GetBytesAsync(mediaId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            FileStreamResult fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(mediaType, fileResult.ContentType);
        }

        [Fact]
        public async Task WhenBytesAreRetrieved_AndModeIsDirect_ThenTheControllerRedirectsToTheSignedUrl()
        {
            // Arrange ----------------------------------------------------------
            Guid mediaId = Guid.NewGuid();
            const string mediaType = "application/pdf";
            const string signedUrl = "https://storage.example/media-signed/abc?sig=xyz";

            IWikiMediaApplicationService service =
                Substitute.For<IWikiMediaApplicationService>();

            service
                .GetMediaForDeliveryAsync(mediaId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMediaRetrievalResult?>(new WikiMediaRetrievalResult
                {
                    Mode = MediaDeliveryMode.Direct,
                    MediaType = mediaType,
                    SignedUrl = signedUrl,
                }));

            WikiMediaController controller = new WikiMediaController(service);

            // Act --------------------------------------------------------------
            IActionResult result = await controller.GetBytesAsync(mediaId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            RedirectResult redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal(signedUrl, redirect.Url);
        }

        [Fact]
        public async Task WhenMediaHandleDoesNotExist_ThenRetrieveReturnsNotFound()
        {
            // Arrange ----------------------------------------------------------
            Guid missingId = Guid.NewGuid();

            IWikiMediaApplicationService service =
                Substitute.For<IWikiMediaApplicationService>();

            service
                .GetMediaForDeliveryAsync(missingId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiMediaRetrievalResult?>(null));

            WikiMediaController controller = new WikiMediaController(service);

            // Act --------------------------------------------------------------
            IActionResult result = await controller.GetBytesAsync(missingId, CancellationToken.None);

            // Assert -----------------------------------------------------------
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Builds an in-memory <see cref="IFormFile"/> over a byte payload so the
        /// controller's upload action can be exercised without an HTTP host.
        /// </summary>
        private static FormFile BuildFormFile(byte[] payload, string contentType, string fileName)
        {
            MemoryStream stream = new MemoryStream(payload);
            return new FormFile(stream, baseStreamOffset: 0, length: payload.Length, name: "file", fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType,
            };
        }
    }
}
