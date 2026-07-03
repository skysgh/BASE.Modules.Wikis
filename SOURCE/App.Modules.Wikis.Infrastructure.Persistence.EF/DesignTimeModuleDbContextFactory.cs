using App.Modules.Wikis;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.DbContexts.Implementations.Base;

namespace App.Modules.Wikis.Infrastructure.Persistence.EF
{
    /// <summary>
    /// Design-time factory for this module's <see cref="ModuleDbContext"/>.
    /// Automatically discovered by <c>dotnet ef migrations</c> tooling.
    /// </summary>
    /// <remarks>
    /// All SQL Server / connection-string / migration-history wiring is
    /// inherited from <see cref="DesignTimeModuleDbContextFactoryBase{TContext}"/>,
    /// so this class is reduced to a single schema-key statement.
    /// </remarks>
    public class DesignTimeModuleDbContextFactory : DesignTimeModuleDbContextFactoryBase<ModuleDbContext>
    {
        /// <inheritdoc/>
        protected override string SchemaKey => ModuleConstants.DbSchemaKey;
    }
}
