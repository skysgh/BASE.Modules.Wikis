using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.Repositories;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="WikiMedia"/> immutable media handles.
    /// </summary>
    /// <remarks>
    /// Domain Repository contract. Extends
    /// <see cref="ICrustStateRepository{TEntity}"/>. Media handles point at
    /// immutable object-store blobs; "replace" means a new handle, never an
    /// in-place mutation.
    /// </remarks>
    public interface IWikiMediaRepository : ICrustStateRepository<WikiMedia>
    {
    }
}
