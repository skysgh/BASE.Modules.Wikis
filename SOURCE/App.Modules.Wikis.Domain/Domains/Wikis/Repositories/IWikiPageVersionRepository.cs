using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.Repositories;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Repositories
{
    /// <summary>
    /// Repository contract for immutable <see cref="WikiPageVersion"/> snapshots.
    /// </summary>
    /// <remarks>
    /// Domain Repository contract. Extends
    /// <see cref="ICrustStateRepository{TEntity}"/>. Versions are write-once by
    /// convention, so callers append new rows rather than mutating existing ones.
    /// </remarks>
    public interface IWikiPageVersionRepository : ICrustStateRepository<WikiPageVersion>
    {
    }
}
