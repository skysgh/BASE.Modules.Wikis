using App.Modules.Wikis.Domain.Domains.Wikis;
using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Permissions;
using App.Modules.Sys.Application.Base;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Domains.Media;
using App.Modules.Sys.Shared.Domains.Media.Services;
using App.Modules.Sys.Shared.ObjectStorage.Models;
using App.Modules.Sys.Shared.ObjectStorage.Models.Enums;
using App.Modules.Sys.Shared.ObjectStorage.Services;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IWikiMediaApplicationService"/>.
    /// </summary>
    public class WikiMediaApplicationService
        : CrustStateAppServiceBase<WikiMedia, WikiMediaReadDto, WikiMediaWriteDto, WikiMediaWriteDto>,
          IWikiMediaApplicationService
    {
        private readonly IMediaUploadInfrastructureService _mediaUploadInfrastructureService;
        private readonly IObjectStorageService _objectStorageService;
        private readonly IWikiAccessAuthorizationService _accessAuthorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiMediaApplicationService"/> class.
        /// </summary>
        /// <param name="repository">The WikiMedia repository for CRUST persistence.</param>
        /// <param name="mapper">The object mapping service for ProjectTo and Map.</param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        /// <param name="mediaUploadInfrastructureService">
        /// The Sys media pipeline used to scan and store media bytes (A4 write half).
        /// </param>
        /// <param name="objectStorageService">
        /// The Sys object store used to stream media bytes back (A4 read half).
        /// </param>
        /// <param name="accessAuthorizationService">
        /// The Application-layer share-based authorization service that gates
        /// every media read/write against the caller's <c>WikiAcl</c> grants.
        /// </param>
        public WikiMediaApplicationService(
            ICrustStateRepository<WikiMedia> repository,
            IObjectMappingService mapper,
            IAppLogger logger,
            IMediaUploadInfrastructureService mediaUploadInfrastructureService,
            IObjectStorageService objectStorageService,
            IWikiAccessAuthorizationService accessAuthorizationService)
            : base(repository, mapper, logger)
        {
            this._mediaUploadInfrastructureService = mediaUploadInfrastructureService;
            this._objectStorageService = objectStorageService;
            this._accessAuthorizationService = accessAuthorizationService;
        }

        /// <summary>
        /// The storage container wiki media is stored in. Wiki media is
        /// ACL-gated, so it lives in the private (signed-access) container,
        /// consistent with the share-based access pattern.
        /// </summary>
        private const StorageContainerType MediaContainer = StorageContainerType.Private;

        /// <inheritdoc/>
        public async Task<WikiMediaReadDto> StoreMediaAsync(
            WikiMediaWriteDto metadata,
            Stream content,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentNullException.ThrowIfNull(content);

            await this.DemandMediaUploadPermissionAsync(metadata.WikiPageFK, cancellationToken)
                .ConfigureAwait(false);

            WikiMedia created = await this.StoreOneAsync(
                    metadata.WikiPageFK,
                    metadata.MediaType,
                    metadata.Title,
                    metadata.Description,
                    sourceMediaFK: null,
                    content,
                    cancellationToken)
                .ConfigureAwait(false);

            return this.ObjectMappingService.Map<WikiMedia, WikiMediaReadDto>(created);
        }

        /// <inheritdoc/>
        public async Task<(WikiMediaReadDto Render, WikiMediaReadDto Source)> StoreDiagramAsync(
            WikiMediaWriteDto metadata,
            Stream source,
            Stream render,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(render);

            // Single share-based gate governs both writes, so a partial pair can
            // never be created by an unauthorized caller.
            await this.DemandMediaUploadPermissionAsync(metadata.WikiPageFK, cancellationToken)
                .ConfigureAwait(false);

            // Store the editable source (mxfile) first so the render can point at
            // it. Media types are fixed to the diagram pair; any type on the DTO
            // is ignored.
            WikiMedia sourceEntity = await this.StoreOneAsync(
                    metadata.WikiPageFK,
                    WikiDomainConstants.DrawioSourceMediaType,
                    metadata.Title,
                    metadata.Description,
                    sourceMediaFK: null,
                    source,
                    cancellationToken)
                .ConfigureAwait(false);

            // Store the display render (SVG) linked back to the source.
            WikiMedia renderEntity = await this.StoreOneAsync(
                    metadata.WikiPageFK,
                    WikiDomainConstants.DrawioRenderMediaType,
                    metadata.Title,
                    metadata.Description,
                    sourceMediaFK: sourceEntity.Id,
                    render,
                    cancellationToken)
                .ConfigureAwait(false);

            WikiMediaReadDto renderDto = this.ObjectMappingService
                .Map<WikiMedia, WikiMediaReadDto>(renderEntity);
            WikiMediaReadDto sourceDto = this.ObjectMappingService
                .Map<WikiMedia, WikiMediaReadDto>(sourceEntity);

            return (renderDto, sourceDto);
        }

        /// <inheritdoc/>
        public async Task<WikiMediaReadDto?> GetDiagramSourceAsync(
            Guid renderMediaId,
            CancellationToken cancellationToken = default)
        {
            // Reuse the share-based read gate; a denied or missing render is
            // indistinguishable so this is never an existence oracle.
            (WikiMedia Handle, string BlobPath)? authorized = await this
                .ResolveAuthorizedHandleAsync(renderMediaId, cancellationToken)
                .ConfigureAwait(false);

            if (authorized is null)
            {
                return null;
            }

            Guid? sourceMediaFK = authorized.Value.Handle.SourceMediaFK;
            if (sourceMediaFK is null)
            {
                // No linked source (e.g. a plain media handle, not a diagram).
                return null;
            }

            // The source belongs to the same page the render was authorized for,
            // so the read grant already covers it; load the handle directly.
            WikiMedia? sourceHandle = await this.Repository
                .GetForUpdateAsync(sourceMediaFK.Value, cancellationToken)
                .ConfigureAwait(false);

            if (sourceHandle is null)
            {
                return null;
            }

            return this.ObjectMappingService.Map<WikiMedia, WikiMediaReadDto>(sourceHandle);
        }

        /// <summary>
        /// Stores one media artifact's bytes through the A4 pipeline and creates
        /// its immutable handle. Shared by <see cref="StoreMediaAsync"/> and
        /// <see cref="StoreDiagramAsync"/> so the allocate → upload → persist
        /// sequence is defined once. The caller is responsible for the
        /// share-based authorization check before invoking this helper.
        /// </summary>
        /// <param name="wikiPageFK">The owning page FK.</param>
        /// <param name="mediaType">The authoritative media (MIME) type to store under.</param>
        /// <param name="title">The handle title.</param>
        /// <param name="description">The handle description.</param>
        /// <param name="sourceMediaFK">
        /// The optional render → source link; <c>null</c> for plain media and for
        /// the source artifact itself.
        /// </param>
        /// <param name="content">The byte stream, positioned at the beginning.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created, persisted media handle.</returns>
        private async Task<WikiMedia> StoreOneAsync(
            Guid wikiPageFK,
            string mediaType,
            string title,
            string description,
            Guid? sourceMediaFK,
            Stream content,
            CancellationToken cancellationToken)
        {
            // Identity and integrity are owned by the store, not the caller:
            // allocate a fresh immutable blob id.
            Guid blobId = Guid.NewGuid();
            string blobPath = WikiMediaBlobPathFactory.BuildBlobPath(blobId, mediaType);
            string fileName = blobId.ToString("D");

            // Push the bytes through the Sys media pipeline (scan -> store). The
            // result carries the authoritative size and content hash.
            MediaUploadResult uploadResult = await this._mediaUploadInfrastructureService
                .UploadAsync(
                    content,
                    fileName,
                    mediaType,
                    MediaContainer,
                    blobPath,
                    metadata: null,
                    cancellationToken)
                .ConfigureAwait(false);

            // Create the immutable handle from the storage result, not the DTO.
            WikiMedia entity = new WikiMedia
            {
                WikiPageFK = wikiPageFK,
                BlobId = blobId,
                MediaType = uploadResult.MediaType,
                ContentHash = uploadResult.ContentHash,
                SourceMediaFK = sourceMediaFK,
                Title = title,
                Description = description,
            };

            return await this.Repository
                .CreateAsync(entity, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Demands permission to upload media to a wiki page.
        /// </summary>
        /// <remarks>
        /// Media upload is allowed either to callers who hold the dedicated
        /// media-management grant or to callers who can author wiki content on
        /// the page. The latter keeps the authoring surface coherent: a caller
        /// allowed to save page content must also be able to attach the media and
        /// diagrams that authored content references.
        /// </remarks>
        /// <param name="wikiPageId">The page the media is being attached to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the current principal lacks both media-management and
        /// content-authoring grants on the page.
        /// </exception>
        private async Task DemandMediaUploadPermissionAsync(
            Guid wikiPageId,
            CancellationToken cancellationToken)
        {
            bool canManageMedia = await this._accessAuthorizationService
                .IsPagePermittedAsync(
                    wikiPageId,
                    WikiPermissionsConfigurationObject.Permissions.ManageMedia,
                    cancellationToken)
                .ConfigureAwait(false);

            if (canManageMedia)
            {
                return;
            }

            bool canAuthor = await this._accessAuthorizationService
                .IsPagePermittedAsync(
                    wikiPageId,
                    WikiPermissionsConfigurationObject.Permissions.Author,
                    cancellationToken)
                .ConfigureAwait(false);

            if (canAuthor)
            {
                return;
            }

            throw new UnauthorizedAccessException(
                "The current principal is not permitted to manage or author media on this page.");
        }

        /// <inheritdoc/>
        public async Task<(Stream Content, string MediaType)?> GetMediaBytesAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default)
        {
            (WikiMedia Handle, string BlobPath)? authorized = await this
                .ResolveAuthorizedHandleAsync(mediaId, cancellationToken)
                .ConfigureAwait(false);

            if (authorized is null)
            {
                return null;
            }

            Stream content = await this._objectStorageService
                .DownloadAsync(MediaContainer, authorized.Value.BlobPath, cancellationToken)
                .ConfigureAwait(false);

            return (content, authorized.Value.Handle.MediaType);
        }

        /// <inheritdoc/>
        public async Task<WikiMediaRetrievalResult?> GetMediaForDeliveryAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default)
        {
            (WikiMedia Handle, string BlobPath)? authorized = await this
                .ResolveAuthorizedHandleAsync(mediaId, cancellationToken)
                .ConfigureAwait(false);

            if (authorized is null)
            {
                return null;
            }

            WikiMedia handle = authorized.Value.Handle;
            string blobPath = authorized.Value.BlobPath;

            // Honour the deployment's configured delivery mode. The default is
            // the CORS-free proxy stream; Direct is opt-in and hands the client
            // a short-lived signed URL so the bytes flow storage -> client
            // without doubling backend bandwidth. Both branches are reached only
            // after the same share-based authorization check above, so they are
            // equally gated.
            if (this._objectStorageService.MediaDeliveryMode == MediaDeliveryMode.Direct)
            {
                SignedUrlResult signed = await this._objectStorageService
                    .GenerateSignedReadUrlAsync(blobPath, ttl: null, cancellationToken)
                    .ConfigureAwait(false);

                return new WikiMediaRetrievalResult
                {
                    Mode = MediaDeliveryMode.Direct,
                    MediaType = handle.MediaType,
                    SignedUrl = signed.Url,
                };
            }

            Stream content = await this._objectStorageService
                .DownloadAsync(MediaContainer, blobPath, cancellationToken)
                .ConfigureAwait(false);

            return new WikiMediaRetrievalResult
            {
                Mode = MediaDeliveryMode.Proxy,
                MediaType = handle.MediaType,
                Content = content,
            };
        }

        /// <summary>
        /// Loads a media handle and applies the share-based read gate, returning
        /// the handle plus its recomputed blob path when the current principal
        /// is permitted, or <c>null</c> when the handle does not exist
        /// <b>or</b> access is denied. The two cases are intentionally
        /// indistinguishable so this method never reveals the existence of media
        /// the caller cannot read.
        /// </summary>
        private async Task<(WikiMedia Handle, string BlobPath)?> ResolveAuthorizedHandleAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            WikiMedia? handle = await this.Repository
                .GetForUpdateAsync(mediaId, cancellationToken)
                .ConfigureAwait(false);

            if (handle is null)
            {
                return null;
            }

            bool permitted = await this._accessAuthorizationService
                .IsPagePermittedAsync(
                    handle.WikiPageFK,
                    WikiPermissionsConfigurationObject.Permissions.Read,
                    cancellationToken)
                .ConfigureAwait(false);

            if (!permitted)
            {
                // Deny as "not found" — never act as an existence/content oracle.
                return null;
            }

            // Recompute the object-store path deterministically from the
            // immutable blob id; the path is never persisted separately.
            string blobPath = WikiMediaBlobPathFactory.BuildBlobPath(handle.BlobId, handle.MediaType);

            return (handle, blobPath);
        }
    }
}
