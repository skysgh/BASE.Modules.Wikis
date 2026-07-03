using App.Modules.Sys.Shared.Repositories;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="WikiNodeStyle"/> additive page-style rows.
    /// </summary>
    /// <remarks>
    /// Extends <see cref="ICrustStateRepository{TEntity}"/> so wiki node styles
    /// participate in the standard CRUST persistence surface.
    /// </remarks>
    public interface IWikiNodeStyleRepository : ICrustStateRepository<WikiNodeStyle>
    {
    }
}
