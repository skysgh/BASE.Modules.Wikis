using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// The bytes-bearing satellite row for the <b>Database</b> body storage sink
    /// (ADR-018N §2.2, Seam 1). It holds the raw text body of exactly one
    /// <see cref="WikiPageVersion"/>, 1:1, via a foreign key <b>into</b> the
    /// version — it never alters the core <see cref="WikiPageVersion"/> row, in
    /// keeping with the ADR-018 §2.7 additive-tables seam.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This entity exists <b>only</b> when the active primary (or a mirror) sink
    /// is <see cref="Enums.WikiBodyStorageSinkKind.Database"/>. The DB sink stores
    /// the body here so it is transactional with the version row and directly
    /// full-text indexable (ADR-018 §3.5 / feature F23) without an external
    /// engine. Other sinks leave this table empty for the version.
    /// </para>
    /// <para>
    /// <b>Immutability.</b> A version is write-once (ADR-018), so its body row is
    /// likewise never updated in place: a new version gets a new body row.
    /// </para>
    /// <para>
    /// <b>Locator linkage.</b> The version's <c>BodyBlobId</c> (reinterpreted as a
    /// sink-agnostic locator, ADR-018N §2.6) equals this row's
    /// <see cref="WikiPageVersionFK"/> under the Database sink, so the coordinator
    /// can resolve the body row directly from the version's stored locator.
    /// </para>
    /// </remarks>
    public class WikiPageVersionBody : DefaultEntityBase
    {
        /// <summary>
        /// FK to the owning <see cref="WikiPageVersion"/> (1:1). Under the
        /// Database sink this also equals the version's body locator
        /// (<c>BodyBlobId</c>), so the body can be fetched directly from the
        /// version's stored locator without a second lookup key.
        /// <para>Navigable, so the suffix is <c>FK</c>, not <c>Id</c>.</para>
        /// </summary>
        public Guid WikiPageVersionFK { get; set; }

        /// <summary>
        /// The raw text body of the version, in the format declared by the
        /// version's <c>ContentFormatKey</c> (ADR-018E). Stored as unbounded
        /// Unicode text so it is full-text indexable in place.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Navigation: the version this body belongs to.
        /// </summary>
        public WikiPageVersion? Version { get; set; }
    }
}
