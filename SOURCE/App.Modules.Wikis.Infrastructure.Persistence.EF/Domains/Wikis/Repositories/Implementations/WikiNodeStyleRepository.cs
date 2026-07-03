using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Wikis.Infrastructure.Persistence.EF;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Repositories.Implementations
{
    /// <summary>
    /// CRUST repository for <see cref="WikiNodeStyle"/> additive page-style rows.
    /// </summary>
    public class WikiNodeStyleRepository : CrustStateRepositoryBase<WikiNodeStyle>, IWikiNodeStyleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiNodeStyleRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics.</param>
        /// <param name="db">The module database context.</param>
        public WikiNodeStyleRepository(IAppLogger logger, ModuleDbContext db)
            : base(logger, db)
        {
        }
    }
}
