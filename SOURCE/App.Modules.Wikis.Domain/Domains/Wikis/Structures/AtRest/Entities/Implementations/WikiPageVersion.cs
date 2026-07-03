using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// An immutable snapshot of a <see cref="WikiPage"/>'s content at a point in
    /// time. This is the heart of the ADR-018 immutability invariant: a version
    /// is <b>never edited in place</b>. "Editing" appends a new
    /// <c>WikiPageVersion</c> and repoints
    /// <see cref="WikiPage.CurrentVersionId"/>.
    /// <para>
    /// The body itself is not stored inline; <see cref="BodyBlobId"/> is the
    /// <b>sink-agnostic body locator</b> (ADR-018N §2.6) addressing the version's
    /// raw body bytes in whichever body-storage sink is configured — a
    /// <c>WikiPageVersionBody</c> satellite row (Database sink), an immutable
    /// object-store blob (ObjectStore sink), or an external content-repo file
    /// (FileSystem sink). <see cref="ContentHash"/> is the sink-independent
    /// content hash of those bytes and is what an Open-Badges / VC endorsement
    /// (ADR-018M) pins to, so a badge can be proven to be "for this exact
    /// version" regardless of where the bytes physically live.
    /// </para>
    /// </summary>
    public class WikiPageVersion : DefaultEntityBase
    {
        /// <summary>
        /// FK to the owning <see cref="WikiPage"/>.
        /// <para>Navigable, so the suffix is <c>FK</c>, not <c>Id</c>.</para>
        /// </summary>
        public Guid WikiPageFK { get; set; }

        /// <summary>
        /// Monotonic version number within the page (1-based). Combined with
        /// <see cref="WikiPageFK"/> it uniquely identifies a revision.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// The <b>sink-agnostic body locator</b> for this version's raw body
        /// bytes (ADR-018N §2.6). Its concrete meaning depends on the active body
        /// sink: the PK of the <c>WikiPageVersionBody</c> satellite row (Database
        /// sink), the object-store blob id (ObjectStore sink), or a deterministic
        /// handle from which the content-repo file path is derived (FileSystem
        /// sink). Replacing content means a new locator on a new version row,
        /// never a mutation of an existing one.
        /// </summary>
        public Guid BodyBlobId { get; set; }

        /// <summary>
        /// The content hash of the body. Computed identically across every body
        /// sink so a mirrored copy can be verified equal; used for staleness/drift
        /// detection and as the subject a verifiable endorsement (ADR-018M) is
        /// pinned to.
        /// </summary>
        public string ContentHash { get; set; } = string.Empty;

        /// <summary>
        /// The declared content format of the body (e.g. markdown), per the
        /// ADR-018E content-format DSL. Stored as a key so the parser-selection
        /// seam can resolve the right parser without a hard enum dependency at
        /// this layer.
        /// </summary>
        public string ContentFormatKey { get; set; } = string.Empty;

        /// <summary>
        /// Navigation: the page this version belongs to.
        /// </summary>
        public WikiPage? Page { get; set; }

        /// <summary>
        /// Navigation: the optional <see cref="WikiPageVersionBody"/> satellite
        /// holding this version's body text when the <b>Database</b> body sink is
        /// active (ADR-018N). <c>null</c> when a non-database sink holds the body
        /// (object store or file system), so this is a zero-or-one association and
        /// adds no column to the immutable version row.
        /// </summary>
        public WikiPageVersionBody? Body { get; set; }
    }
}
