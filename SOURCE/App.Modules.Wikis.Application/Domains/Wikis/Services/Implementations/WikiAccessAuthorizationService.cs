using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Permissions;
using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Sys.Application.Domains.Users.Context.Services;
using App.Modules.Sys.Shared.Configuration;
using App.Modules.Sys.Shared.Domains.AccessControl.Models.Enums;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using Microsoft.EntityFrameworkCore;
using App.Modules.Sys.Infrastructure.Domains.Configuration.Configuration.Services;
using App.Modules.Sys.Infrastructure.Domains.Configuration.Configuration;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IWikiAccessAuthorizationService"/>.
    /// <para>
    /// Resolves the current principal chain (user -> groups -> workspace) and
    /// evaluates <see cref="WikiAcl"/> grants for a required permission. The
    /// page-scoped grant is the more specific override; a wiki-wide grant is the
    /// fallback. <see cref="PrincipalType.Everyone"/> and
    /// <see cref="PrincipalType.Anonymous"/> grants match without an id, so a
    /// resource shared with "everyone" is readable by any caller of the right
    /// audience without an explicit per-principal row.
    /// </para>
    /// </summary>
    public class WikiAccessAuthorizationService : IWikiAccessAuthorizationService
    {
        private readonly IWikiAclRepository _aclRepository;
        private readonly IWikiPageRepository _pageRepository;
        private readonly IUserContextService _userContextService;
        private readonly IPrincipalContextFactory _principalContextFactory;
        private readonly IAppLogger _logger;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="WikiAccessAuthorizationService"/> class.
        /// </summary>
        /// <param name="aclRepository">The wiki ACL repository.</param>
        /// <param name="pageRepository">The wiki page repository (to resolve a page's owning wiki).</param>
        /// <param name="userContextService">The current-user identity context.</param>
        /// <param name="principalContextFactory">
        /// Resolves the current principal chain (user, groups, workspace).
        /// </param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        public WikiAccessAuthorizationService(
            IWikiAclRepository aclRepository,
            IWikiPageRepository pageRepository,
            IUserContextService userContextService,
            IPrincipalContextFactory principalContextFactory,
            IAppLogger logger)
        {
            this._aclRepository = aclRepository;
            this._pageRepository = pageRepository;
            this._userContextService = userContextService;
            this._principalContextFactory = principalContextFactory;
            this._logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> IsPagePermittedAsync(
            Guid wikiPageId,
            string permissionKey,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
            {
                return false;
            }

            // Resolve the page's owning wiki. If the page does not exist we
            // return false (not an exception) so the caller can collapse
            // "missing" and "denied" into a single non-revealing outcome and
            // never act as an existence oracle.
            Guid? wikiId = await this._pageRepository
                .Query()
                .Where(p => p.Id == wikiPageId)
                .Select(p => (Guid?)p.WikiFK)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (wikiId is null)
            {
                return false;
            }

            return await this.EvaluatePermissionAsync(
                    wikiId.Value,
                    wikiPageId,
                    permissionKey,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> IsWikiPermittedAsync(
            Guid wikiId,
            string permissionKey,
            CancellationToken cancellationToken = default)
        {
            if (wikiId == Guid.Empty || string.IsNullOrWhiteSpace(permissionKey))
            {
                return false;
            }

            return await this.EvaluatePermissionAsync(
                    wikiId,
                    null,
                    permissionKey,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Evaluates a permission for the current principal against a wiki root
        /// and, optionally, a specific page within that root.
        /// </summary>
        private async Task<bool> EvaluatePermissionAsync(
            Guid wikiId,
            Guid? wikiPageId,
            string permissionKey,
            CancellationToken cancellationToken)
        {
            // Build the set of (PrincipalType, PrincipalId) pairs the caller
            // operates as: their user id, their groups, and their workspace.
            HashSet<(int PrincipalType, Guid PrincipalId)> heldPrincipals =
                await this.BuildHeldPrincipalsAsync(cancellationToken).ConfigureAwait(false);

            // Load the candidate grants for this permission scoped to either the
            // page (specific override) or its owning wiki (fallback). One query,
            // then evaluate in memory against the principal set.
            List<WikiAcl> grants = await this._aclRepository
                .Query()
                .Where(a => a.PermissionKey == permissionKey
                    && (a.WikiFK == wikiId || (wikiPageId.HasValue && a.WikiPageFK == wikiPageId.Value)))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (grants.Count == 0)
            {
                // Compatibility fallback: until explicit wiki ACL rows exist for
                // a page/wiki, preserve today's open-read authoring flow.
                // Read is allowed anonymously so media byte URLs can render in
                // browser <img> requests (which do not carry SPA auth headers),
                // while non-read permissions still require authentication.
                // Once ACL rows are present, they are authoritative and this
                // fallback no longer applies.
                if (permissionKey == WikiPermissionsConfigurationObject.Permissions.Read)
                {
                    return true;
                }

                return this._userContextService.IsAuthenticated;
            }

            foreach (WikiAcl grant in grants)
            {
                if (this.GrantMatches(grant, heldPrincipals))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Builds the set of principals the current caller operates as, used to
        /// match against issued <see cref="WikiAcl"/> rows.
        /// </summary>
        private async Task<HashSet<(int PrincipalType, Guid PrincipalId)>> BuildHeldPrincipalsAsync(
            CancellationToken cancellationToken)
        {
            HashSet<(int, Guid)> held = new HashSet<(int, Guid)>();

            SettingsPrincipalContext context = await this._principalContextFactory
                .BuildAsync(cancellationToken)
                .ConfigureAwait(false);

            if (context.UserId.HasValue)
            {
                held.Add(((int)PrincipalType.User, context.UserId.Value));
            }

            foreach (Guid groupId in context.GroupIds)
            {
                held.Add(((int)PrincipalType.Group, groupId));
            }

            if (context.WorkspaceId.HasValue)
            {
                held.Add(((int)PrincipalType.Workspace, context.WorkspaceId.Value));
            }

            return held;
        }

        /// <summary>
        /// Determines whether a single grant applies to the current caller.
        /// <see cref="PrincipalType.Everyone"/> matches any authenticated
        /// caller and <see cref="PrincipalType.Anonymous"/> matches any caller
        /// at all, neither requiring a principal-id match.
        /// </summary>
        private bool GrantMatches(
            WikiAcl grant,
            HashSet<(int PrincipalType, Guid PrincipalId)> heldPrincipals)
        {
            if (grant.PrincipalType == (int)PrincipalType.Anonymous)
            {
                return true;
            }

            if (grant.PrincipalType == (int)PrincipalType.Everyone)
            {
                return this._userContextService.IsAuthenticated;
            }

            return heldPrincipals.Contains((grant.PrincipalType, grant.PrincipalId));
        }
    }
}
