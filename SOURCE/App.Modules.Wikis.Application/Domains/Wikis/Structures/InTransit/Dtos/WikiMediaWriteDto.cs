using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Write DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiMedia"/>.
    /// Used for POST (create) operations and serves as the structural base for
    /// <see cref="WikiMediaReadDto"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Media is an immutable blob (ADR-018): "replace" means a new
    /// <see cref="BlobId"/> and a repoint, never an in-place mutation. This row
    /// is the addressable, ACL-able handle for the blob.
    /// </para>
    /// </remarks>
    public class WikiMediaWriteDto : IHasGuidId, IHasTitleAndDescription
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the FK to the owning page this media is attached to.
        /// </summary>
        public Guid WikiPageFK { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the immutable object-store blob
        /// holding the media bytes.
        /// </summary>
        public Guid BlobId { get; set; }

        /// <summary>
        /// Gets or sets the IANA media (MIME) type of the blob, e.g.
        /// <c>image/png</c> or <c>image/svg+xml</c>.
        /// </summary>
        public string MediaType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the content hash of the media blob, used for drift
        /// detection and re-verification of baked endorsement badges.
        /// </summary>
        public string ContentHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional self-referential FK to the <em>source</em>
        /// media artifact this row was rendered from (the draw.io two-artifact
        /// pair). On a render artifact (e.g. the SVG) this points at the editable
        /// source artifact (e.g. the mxfile); on a source artifact or plain media
        /// it is <c>null</c>.
        /// </summary>
        public Guid? SourceMediaFK { get; set; }

        /// <inheritdoc/>
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;
    }
}
