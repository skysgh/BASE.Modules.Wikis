using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiPageVersionBody"/> satellite
    /// that backs the <b>Database</b> body-storage sink (ADR-018N §2.2, Seam 1).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The body row is a <b>zero-or-one</b> dependent of the immutable
    /// <see cref="WikiPageVersion"/>: present when the Database sink holds the
    /// body, absent when an object-store or file-system sink does. The 1:0-1
    /// relationship itself is owned from the principal side in
    /// <see cref="WikiPageVersionEFSchemaTypeConfiguration"/> (the FK lives here,
    /// on the body, so the core <see cref="WikiPageVersion"/> table stays
    /// unaltered as the ADR-018 §2.7 additive-tables seam requires). This config
    /// owns only the body's own columns and its uniqueness.
    /// </para>
    /// <para>
    /// The body is stored as unbounded Unicode text (<c>nvarchar(max)</c>) so it
    /// is full-text indexable in place (ADR-018 §3.5 / feature F23), mirroring the
    /// UISchemas version-content rows. Like its owning version it is write-once by
    /// convention (ADR-018 immutability), enforced at the application layer.
    /// </para>
    /// </remarks>
    public sealed class WikiPageVersionBodyEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiPageVersionBody>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiPageVersionBody> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiPageVersionBodies, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Entity-specific primitives.
            builder.DefineGuid(x => x.WikiPageVersionFK, ref order, isRequired: true);
            // Unbounded Unicode body text: kept FTS-indexable in place (F23).
            builder.DefineString(x => x.Body, ref order, isRequired: true, maxLength: null, optionalColumnType: "nvarchar(max)");

            // Phase 4: Indexes.
            // One body per version: enforce the 1:0-1 with a unique index on the
            // FK. The relationship/navigations are configured from the principal
            // (version) side; here we guarantee at most one body per version.
            builder.HasIndex(e => e.WikiPageVersionFK)
                .IsUnique()
                .HasDatabaseName("IX_WikiPageVersionBodies_WikiPageVersionFK");
        }
    }
}
