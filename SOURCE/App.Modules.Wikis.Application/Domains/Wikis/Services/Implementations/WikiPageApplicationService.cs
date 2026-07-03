using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Application.Base;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Infrastructure.Services.Contracts;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Configuration.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Permissions;
using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IWikiPageApplicationService"/>.
    /// </summary>
    public class WikiPageApplicationService
        : CrustStateAppServiceBase<WikiPage, WikiPageReadDto, WikiPageWriteDto, WikiPageWriteDto>,
          IWikiPageApplicationService
    {
        private readonly IWikiRepository _wikiRepository;
        private readonly IWikiPageVersionRepository _versionRepository;
        private readonly IWikiBodyStoreCoordinator _bodyStoreCoordinator;
        private readonly IAppConfiguration<WikiConfigurationObject> _configuration;
        private readonly IWikiAccessAuthorizationService _accessAuthorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageApplicationService"/> class.
        /// </summary>
        /// <param name="repository">The WikiPage repository for CRUST persistence.</param>
        /// <param name="wikiRepository">The wiki-root repository (resolves the owning wiki's key/tier for body addressing).</param>
        /// <param name="versionRepository">The page-version repository (resolves and appends immutable version snapshots).</param>
        /// <param name="bodyStoreCoordinator">The body-store coordinator that stores and resolves a version's inline body text.</param>
        /// <param name="configuration">The wiki configuration accessor (supplies the default content-format key).</param>
        /// <param name="accessAuthorizationService">The share-based authorization service used to gate wiki authoring.</param>
        /// <param name="mapper">The object mapping service for ProjectTo and Map.</param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        public WikiPageApplicationService(
            ICrustStateRepository<WikiPage> repository,
            IWikiRepository wikiRepository,
            IWikiPageVersionRepository versionRepository,
            IWikiBodyStoreCoordinator bodyStoreCoordinator,
            IAppConfiguration<WikiConfigurationObject> configuration,
            IWikiAccessAuthorizationService accessAuthorizationService,
            IObjectMappingService mapper,
            IAppLogger logger)
            : base(repository, mapper, logger)
        {
            this._wikiRepository = wikiRepository;
            this._versionRepository = versionRepository;
            this._bodyStoreCoordinator = bodyStoreCoordinator;
            this._configuration = configuration;
            this._accessAuthorizationService = accessAuthorizationService;
        }

        /// <inheritdoc/>
        public async Task<WikiPageContentReadDto?> GetContentByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            WikiPage? page = await this.Repository
                .QueryById(id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (page is null)
            {
                return null;
            }

            return await this.ComposeContentAsync(page, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<WikiPageContentReadDto?> GetContentByPathAsync(
            Guid wikiId,
            string path,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            WikiPage? page = await this.Repository
                .Query()
                .FirstOrDefaultAsync(
                    p => p.WikiFK == wikiId && p.Path == path,
                    cancellationToken)
                .ConfigureAwait(false);

            if (page is null)
            {
                return null;
            }

            return await this.ComposeContentAsync(page, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<WikiPageReadDto>> GetPagesByWikiAsync(
            Guid wikiId,
            CancellationToken cancellationToken = default)
        {
            List<WikiPageReadDto> pages = await this.ObjectMappingService
                .ProjectTo<WikiPage, WikiPageReadDto>(
                    this.Repository
                        .Query()
                        .Where(page => page.WikiFK == wikiId)
                        .OrderBy(page => page.Path))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return pages;
        }

        /// <inheritdoc/>
        public async Task<WikiPageContentReadDto> SaveContentAsync(
            WikiPageContentWriteDto request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Path);

            string path = request.Path.Trim();

            // 1. Resolve the page by (wiki, path), or create it (DokuWiki-style
            //    "create this page"). Title/description on the page row are kept
            //    in step with the latest save.
            WikiPage? page = await this.Repository
                .Query()
                .FirstOrDefaultAsync(
                    p => p.WikiFK == request.WikiFK && p.Path == path,
                    cancellationToken)
                .ConfigureAwait(false);

            if (page is null)
            {
                bool canCreateInWiki = await this._accessAuthorizationService
                    .IsWikiPermittedAsync(
                        request.WikiFK,
                        WikiPermissionsConfigurationObject.Permissions.Author,
                        cancellationToken)
                    .ConfigureAwait(false);

                if (!canCreateInWiki)
                {
                    throw new UnauthorizedAccessException(
                        "The current principal is not permitted to author content in this wiki.");
                }
            }
            else
            {
                bool canAuthorPage = await this._accessAuthorizationService
                    .IsPagePermittedAsync(
                        page.Id,
                        WikiPermissionsConfigurationObject.Permissions.Author,
                        cancellationToken)
                    .ConfigureAwait(false);

                if (!canAuthorPage)
                {
                    throw new UnauthorizedAccessException(
                        "The current principal is not permitted to author content on this page.");
                }
            }

            if (page is null)
            {
                page = new WikiPage
                {
                    WikiFK = request.WikiFK,
                    Path = path,
                    Slug = DeriveSlug(path),
                    Title = request.Title,
                    Description = request.Description,
                    Enabled = true,
                };

                page = await this.Repository
                    .CreateAsync(page, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                WikiPage? tracked = await this.Repository
                    .GetForUpdateAsync(page.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (tracked is not null)
                {
                    tracked.Title = request.Title;
                    tracked.Description = request.Description;
                    tracked.Slug = DeriveSlug(path);
                    page = await this.Repository
                        .UpdateAsync(tracked, cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            // 2. Determine the next monotonic version number for this page.
            int lastVersionNumber = await this._versionRepository
                .Query()
                .Where(v => v.WikiPageFK == page.Id)
                .Select(v => (int?)v.VersionNumber)
                .MaxAsync(cancellationToken)
                .ConfigureAwait(false) ?? 0;

            int nextVersionNumber = lastVersionNumber + 1;

            string contentFormatKey = string.IsNullOrWhiteSpace(request.ContentFormatKey)
                ? this._configuration.GetValueOrDefault().DefaultContentFormatKey
                : request.ContentFormatKey.Trim();

            // 3. Build the new immutable version in memory. Its identity is
            //    assigned by the entity base, and under the Database body sink
            //    that same id is the body locator (== BodyBlobId), so the body
            //    row can be addressed before either is saved.
            WikiPageVersion version = new WikiPageVersion
            {
                WikiPageFK = page.Id,
                VersionNumber = nextVersionNumber,
                BodyBlobId = Guid.Empty, // set from the store result's locator below
                ContentFormatKey = contentFormatKey,
            };

            byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(request.Body ?? string.Empty);

            string? wikiKey = await this._wikiRepository
                .QueryById(page.WikiFK)
                .Select(w => w.Key)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            WikiBodyStoreContext context = new WikiBodyStoreContext
            {
                WikiId = page.WikiFK,
                WikiKey = wikiKey ?? string.Empty,
                WikiPageId = page.Id,
                Slug = page.Path,
                WikiPageVersionId = version.Id,
                VersionNumber = version.VersionNumber,
                ContentFormatKey = contentFormatKey,
            };

            // 4. Store the body bytes through the coordinator (primary sink +
            //    best-effort mirrors). The Database sink stages its body row into
            //    the context here and the locator it returns is the version id;
            //    we pin the version row to that sink-agnostic locator and hash.
            WikiBodyStoreResult storeResult = await this._bodyStoreCoordinator
                .StoreBodyAsync(context, bodyBytes, cancellationToken)
                .ConfigureAwait(false);

            version.BodyBlobId = Guid.TryParse(storeResult.BodyLocator, out Guid locatorId)
                ? locatorId
                : version.Id;
            version.ContentHash = storeResult.ContentHash;

            // 5. Persist the version. This save also flushes any body row the
            //    Database sink staged into the context, so the version and its
            //    body commit together (house late-save pattern).
            version = await this._versionRepository
                .CreateAsync(version, cancellationToken)
                .ConfigureAwait(false);

            // 6. Repoint the page's current pointer to the freshly published
            //    version. A page that previously had no published version is
            //    now renderable.
            WikiPage? pageForRepoint = await this.Repository
                .GetForUpdateAsync(page.Id, cancellationToken)
                .ConfigureAwait(false);

            if (pageForRepoint is null)
            {
                // The page was resolved/created moments ago, so a missing tracked
                // row here is a genuine fault worth surfacing rather than silently
                // returning stale content.
                this.LoggingService.LogWarning(
                    "WikiPage {0} could not be re-resolved to repoint its current version {1}; the saved version exists but the page still points at its previous version.",
                    page.Id,
                    version.Id);
            }
            else
            {
                pageForRepoint.CurrentVersionId = version.Id;
                page = await this.Repository
                    .UpdateAsync(pageForRepoint, cancellationToken)
                    .ConfigureAwait(false);
            }

            // 7. Recompose the render projection so the caller renders the saved
            //    state from the same shape a single-GET read returns.
            return await this.ComposeContentAsync(page, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Derives the leaf <see cref="WikiPage.Slug"/> from a canonical
        /// <see cref="WikiPage.Path"/> (the final path segment). The slug is a
        /// non-authoritative convenience; addressing remains by path.
        /// </summary>
        private static string DeriveSlug(string path)
        {
            int lastSeparator = path.LastIndexOf('/');
            return lastSeparator >= 0 && lastSeparator < path.Length - 1
                ? path[(lastSeparator + 1)..]
                : path;
        }

        /// <summary>
        /// Composes the content projection for a resolved page: maps the page
        /// metadata, then resolves the current published version's metadata and
        /// inline body via the body store. A page with no current version is
        /// returned with <see cref="WikiPageContentReadDto.HasContent"/> =
        /// <c>false</c>.
        /// </summary>
        private async Task<WikiPageContentReadDto> ComposeContentAsync(
            WikiPage page,
            CancellationToken cancellationToken)
        {
            WikiPageContentReadDto dto =
                this.ObjectMappingService.Map<WikiPage, WikiPageContentReadDto>(page);

            if (page.CurrentVersionId is null)
            {
                // A fresh or DokuWiki-style "missing" page: addressing exists,
                // no body yet. The client renders the create invitation.
                dto.HasContent = false;
                return dto;
            }

            WikiPageVersion? version = await this._versionRepository
                .QueryById(page.CurrentVersionId.Value)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (version is null)
            {
                // The page points at a version that no longer resolves. This is
                // a data fault, not a normal "missing page", so surface it for
                // maintainers but still return a renderable (empty) projection
                // rather than failing the read.
                this.LoggingService.LogWarning(
                    "WikiPage {0} references current version {1} which could not be resolved; returning empty content.",
                    page.Id,
                    page.CurrentVersionId.Value);
                dto.HasContent = false;
                return dto;
            }

            dto.CurrentVersionId = version.Id;
            dto.VersionNumber = version.VersionNumber;
            dto.ContentFormatKey = version.ContentFormatKey;
            dto.ContentHash = version.ContentHash;

            string? wikiKey = await this._wikiRepository
                .QueryById(page.WikiFK)
                .Select(w => w.Key)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            WikiBodyStoreContext context = new WikiBodyStoreContext
            {
                WikiId = page.WikiFK,
                WikiKey = wikiKey ?? string.Empty,
                WikiPageId = page.Id,
                Slug = page.Path,
                WikiPageVersionId = version.Id,
                VersionNumber = version.VersionNumber,
                ContentFormatKey = version.ContentFormatKey,
            };

            // Under the Database body sink the locator equals the version id
            // (== BodyBlobId); the coordinator parses it back transparently for
            // every sink, so callers never interpret the locator themselves.
            string bodyLocator = version.BodyBlobId.ToString("D");

            ReadOnlyMemory<byte>? bytes = await this._bodyStoreCoordinator
                .GetBodyBytesAsync(context, bodyLocator, cancellationToken)
                .ConfigureAwait(false);

            if (bytes is null)
            {
                this.LoggingService.LogWarning(
                    "WikiPage {0} current version {1} has no resolvable body for locator {2}; returning empty content.",
                    page.Id,
                    version.Id,
                    bodyLocator);
                dto.Body = string.Empty;
                dto.HasContent = false;
                return dto;
            }

            dto.Body = System.Text.Encoding.UTF8.GetString(bytes.Value.Span);
            dto.HasContent = true;
            return dto;
        }
    }
}
