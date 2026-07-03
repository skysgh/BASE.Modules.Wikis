using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiNodeStyle"/> additive page-style entity.
    /// </summary>
    public sealed class WikiNodeStyleEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiNodeStyle>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiNodeStyle> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            int order = 0;

            builder.DefineTable(
                DbSchemaTableNameConstants.WikiNodeStyles,
                DbSchemaSchemaNameConstants.Wikis);

            builder.DefineDefaultEntityBase(ref order);

            builder.DefineGuid(x => x.WikiPageFK, ref order, isRequired: true);
            builder.DefineString(x => x.SectionKey, ref order, isRequired: false);
            builder.DefineString(x => x.BackgroundMediaName, ref order, isRequired: true);
            builder.DefineInt(x => x.OverlayOpacityMode, ref order, isRequired: true);
            builder.DefineInt(x => x.ContrastMode, ref order, isRequired: true);

            builder.DefineRequiredReferenceWithConfiguredFK<WikiNodeStyle, WikiPage>(
                x => x.Page!,
                x => x.WikiPageFK,
                ref order,
                onDelete: DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.WikiPageFK, e.SectionKey })
                .IsUnique();
        }
    }
}
