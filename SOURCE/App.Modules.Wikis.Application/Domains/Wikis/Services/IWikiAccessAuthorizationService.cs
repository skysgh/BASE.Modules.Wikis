using App.Modules.Sys.Shared.Application;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Application-layer authorization service for the wiki domain's
    /// share-based access model.
    /// <para>
    /// <b>Where authorization lives:</b> the wiki domain does not use
    /// transport-level <c>[Authorize]</c> attributes. All access decisions are
    /// made here, in the Application layer, by evaluating the
    /// <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiAcl"/>
    /// grants issued to the current principal (and the principal's groups and
    /// workspace), following the framework's share-based access pattern.
    /// </para>
    /// <para>
    /// <b>No content oracle:</b> resolvers and callers must treat a denied
    /// check the same whether the target does not exist or the reader simply
    /// cannot see it — never reveal existence of content the reader has no
    /// grant for.
    /// </para>
    /// </summary>
    public interface IWikiAccessAuthorizationService : IHasApplicationScopeService
    {
        /// <summary>
        /// Determines whether the current principal holds the given permission
        /// on the specified page, falling back to a wiki-wide grant when no
        /// page-scoped grant is present.
        /// </summary>
        /// <param name="wikiPageId">The page the permission is required on.</param>
        /// <param name="permissionKey">
        /// The required permission key, e.g.
        /// <see cref="Domain.Domains.Wikis.Permissions.WikiPermissionsConfigurationObject.Permissions.Read"/>.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// <c>true</c> when the current principal is granted the permission;
        /// otherwise <c>false</c>. Returns <c>false</c> (not throwing) when the
        /// page does not exist, so callers can collapse "missing" and "denied"
        /// into a single non-revealing result.
        /// </returns>
        Task<bool> IsPagePermittedAsync(
            Guid wikiPageId,
            string permissionKey,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the current principal holds the given permission
        /// on the specified wiki root.
        /// </summary>
        /// <param name="wikiId">The wiki root the permission is required on.</param>
        /// <param name="permissionKey">
        /// The required permission key, e.g.
        /// <see cref="Domain.Domains.Wikis.Permissions.WikiPermissionsConfigurationObject.Permissions.Author"/>.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// <c>true</c> when the current principal is granted the permission;
        /// otherwise <c>false</c>. Returns <c>false</c> (not throwing) when the
        /// wiki does not exist, so callers can collapse "missing" and "denied"
        /// into a single non-revealing result.
        /// </returns>
        Task<bool> IsWikiPermittedAsync(
            Guid wikiId,
            string permissionKey,
            CancellationToken cancellationToken = default);
    }
}
