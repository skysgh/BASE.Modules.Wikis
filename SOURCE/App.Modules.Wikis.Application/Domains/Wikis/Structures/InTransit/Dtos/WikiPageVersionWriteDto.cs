using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Write DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPageVersion"/>.
    /// Used for POST (create) operations and serves as the structural base for
    /// <see cref="WikiPageVersionReadDto"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A version is an immutable content snapshot (ADR-018): once created it is
    /// never edited in place. The body bytes live in an object-store blob
    /// referenced by <see cref="BodyBlobId"/>; <see cref="ContentHash"/> is what
    /// a verifiable endorsement (ADR-018M) pins to.
    /// </para>
    /// </remarks>
    public class WikiPageVersionWriteDto : IHasGuidId
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the FK to the owning page.
        /// </summary>
        public Guid WikiPageFK { get; set; }

        /// <summary>
        /// Gets or sets the monotonic version number within the page (1-based).
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the immutable object-store blob
        /// holding this version's raw body bytes.
        /// </summary>
        public Guid BodyBlobId { get; set; }

        /// <summary>
        /// Gets or sets the content hash of the body blob, used for drift
        /// detection and as the subject a verifiable endorsement is pinned to.
        /// </summary>
        public string ContentHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the declared content format key of the body (e.g.
        /// markdown), per the ADR-018E content-format DSL.
        /// </summary>
        public string ContentFormatKey { get; set; } = string.Empty;
    }
}
