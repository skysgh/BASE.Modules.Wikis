using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiTemplateBinding"/> entity
    /// (ADR-018C namespace/subtree template attachment).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The owning template relationship is configured from the principal side in
    /// <see cref="WikiTemplateEFSchemaTypeConfiguration"/>. Here we own the
    /// binding's own columns and its optional <see cref="WikiPage"/> scope.
    /// </para>
    /// <para>
    /// <see cref="WikiTemplateBinding.WikiId"/> is a non-navigable aggregate id,
    /// not a relationship: the binding already cascades from its template (which
    /// the wiki owns), and adding a second <c>Wiki -&gt; Binding</c> cascade would
    /// create multiple cascade paths SQL Server rejects. The optional
    /// page-subtree scope uses <see cref="DeleteBehavior.SetNull"/> so deleting a
    /// scoped page simply widens the binding rather than deleting it.
    /// </para>
    /// </remarks>
    public sealed class WikiTemplateBindingEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiTemplateBinding>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiTemplateBinding> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiTemplateBindings, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Contract-based columns.
            builder.DefineIHasEnabled(ref order);
            builder.DefineIHasPrecedenceOrder(ref order);

            // Phase 4: Entity-specific primitives.
            builder.DefineGuid(x => x.WikiTemplateFK, ref order, isRequired: true);
            // Non-navigable scope/query key (see remarks): plain Guid to avoid a
            // second cascade path into this table.
            builder.DefineGuid(x => x.WikiId, ref order, isRequired: true);
            builder.DefineString(x => x.ScopeSlugPrefix, ref order, isRequired: false);

            // Phase 5: Relationships.
            // Optional page-subtree scope. WikiPage carries no inverse collection
            // of bindings, so use the no-inverse helper; SetNull on delete.
            builder.DefineOptionalReferenceWithConfiguredFK<WikiTemplateBinding, WikiPage>(
                b => b.ScopePage,
                b => b.ScopeWikiPageFK,
                ref order,
                onDelete: DeleteBehavior.SetNull);

            // Phase 6: Indexes.
            builder.HasIndex(e => new { e.WikiId, e.PrecedenceOrder })
                .HasDatabaseName("IX_WikiTemplateBindings_WikiId_PrecedenceOrder");
        }
    }
}
