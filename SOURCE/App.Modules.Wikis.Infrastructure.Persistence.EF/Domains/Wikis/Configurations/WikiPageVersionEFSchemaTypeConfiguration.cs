using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the immutable <see cref="WikiPageVersion"/>
    /// snapshot entity.
    /// </summary>
    /// <remarks>
    /// The owning page relationship is configured from the principal side in
    /// <see cref="WikiPageEFSchemaTypeConfiguration"/>. Here we own the version's
    /// own columns: the body locator pointer, content hash, declared format, and
    /// version number — plus the optional 1:0-1 to its
    /// <see cref="WikiPageVersionBody"/> (the ADR-018N Database body sink; absent
    /// for non-database sinks). None of these columns are ever updated in place —
    /// the row is write-once by convention (ADR-018 immutability), enforced at the
    /// application layer rather than by a DB trigger.
    /// </remarks>
    public sealed class WikiPageVersionEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiPageVersion>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiPageVersion> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiPageVersions, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Entity-specific primitives.
            builder.DefineGuid(x => x.WikiPageFK, ref order, isRequired: true);
            builder.DefineInt(x => x.VersionNumber, ref order, isRequired: true);
            builder.DefineGuid(x => x.BodyBlobId, ref order, isRequired: true);
            builder.DefineString(x => x.ContentHash, ref order, isRequired: true);
            builder.DefineString(x => x.ContentFormatKey, ref order, isRequired: true);

            // Phase 4: Relationships.
            // 1:0-1 WikiPageVersion -> WikiPageVersionBody (ADR-018N Database body
            // sink). The body is the OPTIONAL dependent: present only when the DB
            // sink holds this version's body, absent for object-store/file-system
            // sinks. The FK lives on the body, so the version table is unaltered
            // (ADR-018 §2.7). Cascade so a version's DB body row dies with it.
            builder.DefineOneToZeroOrOne<WikiPageVersion, WikiPageVersionBody>(
                v => v.Body,
                b => b.Version,
                b => b.WikiPageVersionFK,
                onDelete: DeleteBehavior.Cascade);

            // Phase 5: Indexes.
            // (PageFK, VersionNumber) is the natural key of a revision.
            builder.HasIndex(e => new { e.WikiPageFK, e.VersionNumber })
                .IsUnique()
                .HasDatabaseName("IX_WikiPageVersions_WikiPageFK_VersionNumber");
        }
    }
}
