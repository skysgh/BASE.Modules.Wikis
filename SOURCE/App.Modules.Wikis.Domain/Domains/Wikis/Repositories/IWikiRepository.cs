using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.Repositories;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="Wiki"/> roots.
    /// </summary>
    /// <remarks>
    /// Domain Repository contract (not an Application Service contract).
    /// Extends <see cref="ICrustStateRepository{TEntity}"/> for standard CRUST
    /// (Create, Read, Update, State-Transition) persistence operations. The
    /// concrete implementation lives in Infrastructure.Persistence.EF and is
    /// injected into the corresponding Application Service.
    /// </remarks>
    public interface IWikiRepository : ICrustStateRepository<Wiki>
    {
    }
}
