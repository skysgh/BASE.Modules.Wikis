using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Wikis.Infrastructure.Persistence.EF;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Repositories.Implementations
{
    /// <summary>
    /// CRUST repository for <see cref="WikiPage"/> entities.
    /// </summary>
    public class WikiPageRepository : CrustStateRepositoryBase<WikiPage>, IWikiPageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics.</param>
        /// <param name="db">The module database context.</param>
        public WikiPageRepository(IAppLogger logger, ModuleDbContext db)
            : base(logger, db)
        {
        }
    }
}
