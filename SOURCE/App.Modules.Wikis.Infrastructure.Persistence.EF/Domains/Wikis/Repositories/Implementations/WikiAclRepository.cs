using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Wikis.Infrastructure.Persistence.EF;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Repositories.Implementations
{
    /// <summary>
    /// CRUST repository for <see cref="WikiAcl"/> share-based access-control
    /// entries.
    /// </summary>
    public class WikiAclRepository : CrustStateRepositoryBase<WikiAcl>, IWikiAclRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiAclRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics.</param>
        /// <param name="db">The module database context.</param>
        public WikiAclRepository(IAppLogger logger, ModuleDbContext db)
            : base(logger, db)
        {
        }
    }
}
