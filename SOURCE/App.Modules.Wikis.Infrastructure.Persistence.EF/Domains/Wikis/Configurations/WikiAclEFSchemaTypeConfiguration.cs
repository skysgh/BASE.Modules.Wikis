using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiAcl"/> share-based
    /// access-control entry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An ACL row is scoped to either a whole <see cref="Wiki"/> or a single
    /// <see cref="WikiPage"/> (exactly one of the two scope FKs is populated),
    /// so both relationships are configured as <b>optional</b> references whose
    /// intent is to clear the dangling scope pointer when the target is removed,
    /// rather than cascade-delete grants that may still describe the other
    /// scope. The principal target itself
    /// (<see cref="WikiAcl.PrincipalId"/>/<see cref="WikiAcl.PrincipalType"/>)
    /// is a plain value pair per the share-based access pattern — principals
    /// live in other modules and are not navigated here.
    /// </para>
    /// <para>
    /// SQL Server rejects two database-level referential-action paths that both
    /// terminate at the same table. Here those paths are the direct
    /// <c>Wiki -&gt; WikiAcl.WikiFK</c> link and the indirect
    /// <c>Wiki -&gt; WikiPage (cascade) -&gt; WikiAcl.WikiPageFK</c> link, which
    /// triggers "may cause cycles or multiple cascade paths". To resolve this
    /// the direct wiki scope uses <see cref="DeleteBehavior.ClientSetNull"/>:
    /// the foreign key is emitted as <c>ON DELETE NO ACTION</c> (eliminating the
    /// second database path) while EF still nulls <see cref="WikiAcl.WikiFK"/>
    /// for tracked grants at save time, preserving the set-null intent. The page
    /// scope keeps database-level <see cref="DeleteBehavior.SetNull"/> because it
    /// is the sole action path reaching the ACL table through pages.
    /// </para>
    /// </remarks>
    public sealed class WikiAclEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiAcl>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiAcl> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiAcls, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Entity-specific primitives.
            builder.DefineGuid(x => x.PrincipalId, ref order, isRequired: true);
            builder.DefineInt(x => x.PrincipalType, ref order, isRequired: true);
            builder.DefineString(x => x.PermissionKey, ref order, isRequired: true);

            // Phase 4: Relationships — optional, mutually-exclusive scopes.
            // Direct wiki scope uses ClientSetNull (emitted as ON DELETE NO ACTION)
            // to avoid a second SQL Server cascade path into WikiAcls; EF still
            // nulls WikiFK for tracked grants at save time.
            builder.DefineOptionalReference<WikiAcl, Wiki>(
                a => a.Wiki,
                w => w.Acls,
                a => a.WikiFK,
                onDelete: DeleteBehavior.ClientSetNull);

            // Page scope is the sole action path reaching WikiAcls through pages,
            // so it keeps database-level SetNull.
            builder.DefineOptionalReference<WikiAcl, WikiPage>(
                a => a.Page,
                p => p.Acls,
                a => a.WikiPageFK,
                onDelete: DeleteBehavior.SetNull);

            // Phase 5: Indexes.
            // Fast lookup of all grants for a principal when resolving access.
            builder.HasIndex(e => new { e.PrincipalId, e.PrincipalType })
                .HasDatabaseName("IX_WikiAcls_PrincipalId_PrincipalType");
        }
    }
}
