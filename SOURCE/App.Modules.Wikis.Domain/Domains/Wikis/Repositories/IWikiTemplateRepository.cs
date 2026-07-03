using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.Repositories;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="WikiTemplate"/> entities (ADR-018C).
    /// </summary>
    /// <remarks>
    /// Domain Repository contract. Extends
    /// <see cref="ICrustStateRepository{TEntity}"/> for standard CRUST
    /// persistence operations; the EF implementation is injected into the
    /// corresponding Application Service.
    /// </remarks>
    public interface IWikiTemplateRepository : ICrustStateRepository<WikiTemplate>
    {
    }
}
