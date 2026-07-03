using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="Wiki"/> root entity.
    /// </summary>
    /// <remarks>
    /// A <c>Wiki</c> is the principal of a 1-* to <see cref="WikiPage"/>. The
    /// page tree itself (parent/child) is configured on
    /// <see cref="WikiPageEFSchemaTypeConfiguration"/>, so here we only own the
    /// root's own columns and its top-level pages collection.
    /// </remarks>
    public sealed class WikiEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<Wiki>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Wiki> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.Wikis, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity (Id + audit + timestamp + record state).
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Contract-based columns.
            builder.DefineIHasKey(ref order);
            builder.DefineIHasTitleAndDescription(ref order);
            builder.DefineIHasEnabled(ref order);

            // Phase 4: Entity-specific primitives.
            // Optional owning workspace; aggregate-id helper keeps it a plain,
            // non-navigable Guid? (workspaces live in another module).
            builder.DefineOptionalAggregateId(x => x.OwnerWorkspaceId, ref order);

            // Phase 5: Relationships.
            // 1-* Wiki -> WikiPages. The dependent's FK (WikiFK) is required;
            // deleting a wiki cascades to its pages.
            builder.DefineOneToZeroOrManyRequired<Wiki, WikiPage>(
                w => w.Pages,
                p => p.Wiki!,
                p => p.WikiFK,
                onDelete: DeleteBehavior.Cascade);

            // Phase 6: Indexes.
            // The mount key must be unique so wiki:{key}:{slug} resolves a single root.
            builder.HasIndex(e => e.Key)
                .IsUnique()
                .HasDatabaseName("IX_Wikis_Key");
        }
    }
}
