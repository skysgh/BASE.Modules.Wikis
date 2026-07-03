using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Shared.Repositories;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="WikiAcl"/> share-based access-control
    /// entries.
    /// </summary>
    /// <remarks>
    /// Domain Repository contract. Extends
    /// <see cref="ICrustStateRepository{TEntity}"/>. Used by resolvers to load a
    /// principal's grants; must never be exposed in a way that turns into an
    /// existence/content oracle for content the reader cannot see.
    /// </remarks>
    public interface IWikiAclRepository : ICrustStateRepository<WikiAcl>
    {
    }
}
