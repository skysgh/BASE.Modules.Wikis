using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using App.Modules.Wikis.Infrastructure.Persistence.EF;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Repositories.Implementations
{
    /// <summary>
    /// CRUST repository for <see cref="WikiMedia"/> immutable media handles.
    /// </summary>
    public class WikiMediaRepository : CrustStateRepositoryBase<WikiMedia>, IWikiMediaRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiMediaRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics.</param>
        /// <param name="db">The module database context.</param>
        public WikiMediaRepository(IAppLogger logger, ModuleDbContext db)
            : base(logger, db)
        {
        }
    }
}
