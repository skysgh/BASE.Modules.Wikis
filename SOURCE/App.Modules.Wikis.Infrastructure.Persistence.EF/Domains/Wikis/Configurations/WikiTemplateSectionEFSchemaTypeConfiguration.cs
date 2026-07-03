using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiTemplateSection"/> entity
    /// (ADR-018C ordered scaffold/lint section).
    /// </summary>
    /// <remarks>
    /// The owning template relationship is configured from the principal side in
    /// <see cref="WikiTemplateEFSchemaTypeConfiguration"/>, so here we only own
    /// the section's own columns. <see cref="WikiTemplateSection.PrecedenceOrder"/>
    /// is logic ordering (it fixes both the scaffold emission order and the lint
    /// walk order), so it is persisted via the <c>IHasPrecedenceOrder</c>
    /// contract column rather than a display-hint column.
    /// </remarks>
    public sealed class WikiTemplateSectionEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiTemplateSection>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiTemplateSection> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiTemplateSections, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Contract-based columns.
            builder.DefineIHasKey(ref order);
            builder.DefineIHasTitleAndDescription(ref order);
            builder.DefineIHasPrecedenceOrder(ref order);

            // Phase 4: Entity-specific primitives.
            builder.DefineGuid(x => x.WikiTemplateFK, ref order, isRequired: true);
            builder.DefineBool(x => x.IsRequired, ref order);
            builder.DefineString(x => x.PlaceholderBody, ref order, isRequired: false);

            // Phase 5: Indexes.
            // Section key is unique within a template.
            builder.HasIndex(e => new { e.WikiTemplateFK, e.Key })
                .IsUnique()
                .HasDatabaseName("IX_WikiTemplateSections_WikiTemplateFK_Key");
        }
    }
}
