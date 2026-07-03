using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiTemplate"/> aggregate
    /// (ADR-018C templates-as-pages).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A template is the principal of two 1-* relationships it owns here: its
    /// ordered <see cref="WikiTemplateSection"/> scaffold blocks and its
    /// <see cref="WikiTemplateBinding"/> attachments. Both cascade on delete —
    /// removing a template removes its sections and bindings, since neither has
    /// meaning without the template.
    /// </para>
    /// <para>
    /// The owning <see cref="Wiki"/> relationship is configured here from the
    /// dependent side via <c>DefineRequiredReferenceWithConfiguredFK</c> with
    /// <see cref="DeleteBehavior.Restrict"/>: the wiki root has no inverse
    /// collection of templates, and Restrict avoids a second cascade path into
    /// the section/binding tables (SQL Server forbids multiple cascade paths).
    /// </para>
    /// </remarks>
    public sealed class WikiTemplateEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiTemplate>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiTemplate> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiTemplates, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Contract-based columns.
            builder.DefineIHasKey(ref order);
            builder.DefineIHasTitleAndDescription(ref order);
            builder.DefineIHasEnabled(ref order);

            // Phase 4: Entity-specific primitives.
            builder.DefineString(x => x.ContentFormatKey, ref order, isRequired: true);

            // Phase 5: Relationships.
            // Owning wiki root. Restrict (not Cascade) so we do not create a
            // second cascade path Wiki -> Template -> Sections/Bindings.
            builder.DefineRequiredReferenceWithConfiguredFK<WikiTemplate, Wiki>(
                t => t.Wiki!,
                t => t.WikiFK,
                ref order,
                onDelete: DeleteBehavior.Restrict);

            // 1-* WikiTemplate -> Sections. Required FK; cascade on delete.
            builder.DefineOneToZeroOrManyRequired<WikiTemplate, WikiTemplateSection>(
                t => t.Sections,
                s => s.Template!,
                s => s.WikiTemplateFK,
                onDelete: DeleteBehavior.Cascade);

            // 1-* WikiTemplate -> Bindings. Required FK; cascade on delete.
            builder.DefineOneToZeroOrManyRequired<WikiTemplate, WikiTemplateBinding>(
                t => t.Bindings,
                b => b.Template!,
                b => b.WikiTemplateFK,
                onDelete: DeleteBehavior.Cascade);

            // Phase 6: Indexes.
            // Template key is unique within a wiki root.
            builder.HasIndex(e => new { e.WikiFK, e.Key })
                .IsUnique()
                .HasDatabaseName("IX_WikiTemplates_WikiFK_Key");
        }
    }
}
