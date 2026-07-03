using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// A media asset that lives in the vicinity of a <see cref="WikiPage"/>
    /// (images, diagrams, baked badge images, draw.io SVG, etc.). Like page
    /// bodies, media is an <b>immutable blob</b> (ADR-018): "replace" means a
    /// new <see cref="BlobId"/> and a repoint, never an in-place mutation.
    /// <para>
    /// This row is the addressable, ACL-able handle for the blob; the bytes
    /// themselves are held by the object store via <c>Sys.Infrastructure.Media</c>.
    /// </para>
    /// </summary>
    public class WikiMedia : DefaultEntityBase, IHasTitleAndDescription
    {
        /// <summary>
        /// FK to the owning <see cref="WikiPage"/> this media is attached to.
        /// <para>Navigable, so the suffix is <c>FK</c>, not <c>Id</c>.</para>
        /// </summary>
        public Guid WikiPageFK { get; set; }

        /// <summary>
        /// Identifier of the immutable object-store blob holding the media
        /// bytes.
        /// </summary>
        public Guid BlobId { get; set; }

        /// <summary>
        /// The IANA media (MIME) type of the blob, e.g. <c>image/png</c> or
        /// <c>image/svg+xml</c>.
        /// </summary>
        public string MediaType { get; set; } = string.Empty;

        /// <summary>
        /// The content hash of the media blob, used for drift detection and to
        /// support re-verification of baked endorsement badges (ADR-018M).
        /// </summary>
        public string ContentHash { get; set; } = string.Empty;

        /// <summary>
        /// Optional self-referential FK to the <em>source</em> media artifact
        /// this row was rendered from, used for the draw.io two-artifact pair
        /// (ADR-018, §10 of the body-storage implementation note).
        /// <para>
        /// On a <em>render</em> artifact (e.g. a flattened
        /// <c>image/svg+xml</c>, see
        /// <see cref="Constants.WikiDomainConstants.DrawioRenderMediaType"/>)
        /// this points at the editable <em>source</em> artifact (e.g. the
        /// <c>application/vnd.jgraph.mxfile</c>, see
        /// <see cref="Constants.WikiDomainConstants.DrawioSourceMediaType"/>) the
        /// editor reopens for edit. On a source artifact (or any plain media)
        /// this is <c>null</c>.
        /// </para>
        /// <para>
        /// Navigable, so the suffix is <c>FK</c>, not <c>Id</c>. Keeping both
        /// artifacts as <see cref="WikiMedia"/> rows preserves a single ACL
        /// surface, a single immutable-blob lifecycle, and a single storage path
        /// derivation for the diagram pair.
        /// </para>
        /// </summary>
        public Guid? SourceMediaFK { get; set; }

        /// <inheritdoc />
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Navigation: the page this media is attached to.
        /// </summary>
        public WikiPage? Page { get; set; }

        /// <summary>
        /// Navigation: the source media artifact this render was produced from,
        /// resolved from <see cref="SourceMediaFK"/>. <c>null</c> on source
        /// artifacts and on plain (non-paired) media.
        /// </summary>
        public WikiMedia? Source { get; set; }
    }
}
