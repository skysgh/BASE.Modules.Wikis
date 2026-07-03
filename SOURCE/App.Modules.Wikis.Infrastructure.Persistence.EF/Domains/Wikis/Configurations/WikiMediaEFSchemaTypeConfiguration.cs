using App.Modules.Wikis.Infrastructure.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="WikiMedia"/> entity (an
    /// immutable media blob handle attached to a page).
    /// </summary>
    /// <remarks>
    /// A page owns many media handles. We configure the relationship from the
    /// dependent side here using the page FK, with a cascade delete so a
    /// deleted page takes its media handles with it (the underlying object-store
    /// blobs are reaped separately by the media subsystem).
    /// <para>
    /// A media handle may also <em>optionally</em> reference another media
    /// handle of the same type as its <c>Source</c> — the draw.io two-artifact
    /// pair (ADR-018, §10 of the body-storage implementation note). A render
    /// (SVG) points at the editable source (mxfile). This self-reference uses
    /// <see cref="DeleteBehavior.NoAction"/>: SQL Server forbids a cascade or
    /// set-null on this self-edge because the required page cascade-delete path
    /// already converges on <c>WikiMedia</c> (multiple cascade paths). Both
    /// artifacts of a pair share a page, so the page cascade reaps them
    /// together; individually unpairing a render from its source is an
    /// application-layer concern.
    /// </para>
    /// </remarks>
    public sealed class WikiMediaEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<WikiMedia>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WikiMedia> builder)
        {
            int order = 0;

            // Phase 1: Table identity.
            builder.DefineTable(DbSchemaTableNameConstants.WikiMedia, DbSchemaSchemaNameConstants.Wikis);

            // Phase 2: Base entity.
            builder.DefineDefaultEntityBase(ref order);

            // Phase 3: Contract-based columns.
            builder.DefineIHasTitleAndDescription(ref order);

            // Phase 4: Entity-specific primitives.
            builder.DefineGuid(x => x.BlobId, ref order, isRequired: true);
            builder.DefineString(x => x.MediaType, ref order, isRequired: true);
            builder.DefineString(x => x.ContentHash, ref order, isRequired: true);

            // Phase 5: Relationship — dependent side of WikiPage 1-* WikiMedia.
            builder.DefineRequiredReference<WikiMedia, WikiPage>(
                m => m.Page!,
                p => p.Media,
                m => m.WikiPageFK,
                onDelete: DeleteBehavior.Cascade);

            // Phase 5b: Optional self-reference — render -> source (draw.io pair).
            // NoAction: the required WikiPage -> WikiMedia cascade above already
            // reaps both artifacts of a pair together (they share a page), so the
            // database needs no cascade on this edge. SQL Server forbids SetNull
            // here because the WikiPage cascade-delete path and a self-referential
            // set-null path converge on WikiMedia (multiple cascade paths), so the
            // self-edge must be NoAction. Individually unpairing a render from its
            // source (without deleting the page) is handled in the application
            // layer. The principal (source) side intentionally has no inverse
            // collection.
            builder.DefineOptionalReferenceWithConfiguredFK<WikiMedia, WikiMedia>(
                m => m.Source,
                m => m.SourceMediaFK,
                ref order,
                onDelete: DeleteBehavior.NoAction,
                optionalIndexName: "IX_WikiMedia_SourceMediaFK");

            // Phase 6: Indexes.
            builder.HasIndex(e => e.WikiPageFK)
                .HasDatabaseName("IX_WikiMedia_WikiPageFK");
        }
    }
}
