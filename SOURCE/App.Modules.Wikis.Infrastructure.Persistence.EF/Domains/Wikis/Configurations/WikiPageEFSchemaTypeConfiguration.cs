using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiPage"/> entity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This config owns two structural relationships:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// The <b>self-referencing page tree</b> (Parent/Children via
    /// <see cref="WikiPage.ParentWikiPageFK"/>). We deliberately use
    /// <c>DeleteBehavior.Restrict</c> here so a parent cannot be deleted out
    /// from under its children — moving/re-parenting is an explicit operation,
    /// not a side effect of a cascade.
    /// </item>
    /// <item>
    /// The <b>1-* to immutable versions</b>. Deleting a page cascades to its
    /// version history.
    /// </item>
    /// </list>
    /// <para>
    /// The owning <see cref="Wiki"/> relationship is configured from the
    /// principal side in <see cref="WikiEFSchemaTypeConfiguration"/>, so it is
    /// not repeated here.
    /// </para>
    /// </remarks>
    public sealed class WikiPageEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiPage>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiPage> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiPages, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Contract-based columns.
            builder.DefineIHasTitleAndDescription(ref order);
            builder.DefineIHasEnabled(ref order);

            // Phase 4: Entity-specific primitives.
            builder.DefineGuid(x => x.WikiFK, ref order, isRequired: true);
            // Path is the canonical addressing source of truth (DokuWiki-style);
            // Slug is its derived last segment. Both are persisted: Path drives
            // uniqueness and prefix search, Slug remains for routing/cross-links.
            builder.DefineString(x => x.Path, ref order, isRequired: true);
            builder.DefineString(x => x.Slug, ref order, isRequired: true);
            // Current published version is optional (a new page has none yet);
            // kept as a plain Guid? aggregate id to avoid an FK cycle with the
            // version table (the version already points at the page).
            builder.DefineOptionalAggregateId(x => x.CurrentVersionId, ref order);

            // Phase 5: Relationships.
            // Self-referencing page tree. Restrict delete: re-parent explicitly.
            builder.DefineSelfReferenceHierarchy(
                p => p.Parent,
                p => p.Children,
                p => p.ParentWikiPageFK,
                onDelete: DeleteBehavior.Restrict);

            // 1-* WikiPage -> WikiPageVersions. Required FK; cascade on delete.
            builder.DefineOneToZeroOrManyRequired<WikiPage, WikiPageVersion>(
                p => p.Versions,
                v => v.Page!,
                v => v.WikiPageFK,
                onDelete: DeleteBehavior.Cascade);

            // Phase 6: Indexes.
            // Path is the canonical address: unique within a wiki root.
            builder.HasIndex(e => new { e.WikiFK, e.Path })
                .IsUnique()
                .HasDatabaseName("IX_WikiPages_WikiFK_Path");

            // Path prefix-search support (children = Path LIKE 'prefix/%').
            builder.HasIndex(e => e.Path)
                .HasDatabaseName("IX_WikiPages_Path");

            // Slug is the derived leaf; multiple pages may share a leaf name
            // under different prefixes, so this index is a non-unique lookup
            // aid only (uniqueness is enforced on Path above).
            builder.HasIndex(e => new { e.WikiFK, e.Slug })
                .HasDatabaseName("IX_WikiPages_WikiFK_Slug");
        }
    }
}
