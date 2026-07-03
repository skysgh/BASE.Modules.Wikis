namespace App.Modules.Wikis.Domain.Domains.Wikis.Enums
{
    /// <summary>
    /// Identifies a wiki page-version <em>body</em> storage sink — the concrete
    /// place a version's text body bytes are persisted to and read from
    /// (ADR-018N).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A version body is authored wiki content (markdown / HTML / asciidoc per
    /// the ADR-018E content-format DSL). Unlike a media asset (which is always an
    /// immutable object-store blob), the body's location is an administrator
    /// choice, surfaced through
    /// <see cref="Configuration.Implementations.WikiConfigurationObject.BodyStoragePrimarySink"/>
    /// and the mirror-sink list, so the DB-vs-blob-vs-file decision can be made
    /// per environment without forking the core wiki model.
    /// </para>
    /// <para>
    /// Each sink preserves the ADR-018 immutability invariant: a body write
    /// always produces a new locator, never an in-place mutation. The body
    /// remains addressed by the version row (<c>BodyBlobId</c> reinterpreted as a
    /// sink-agnostic locator, ADR-018N §2.6); switching sinks therefore never
    /// touches stored content references.
    /// </para>
    /// <para>
    /// Follows the framework enum convention: the first four members are the
    /// reserved sentinels and real options begin at <c>4</c>.
    /// </para>
    /// </remarks>
    public enum WikiBodyStorageSinkKind
    {
        /// <summary>No value assigned.</summary>
        Undefined = 0,

        /// <summary>Not applicable in this context.</summary>
        NotApplicable = 1,

        /// <summary>Sink is unspecified.</summary>
        Unspecified = 2,

        /// <summary>Sink is not known.</summary>
        Unknown = 3,

        /// <summary>
        /// Store the body in a <c>WikiPageVersionBody</c> satellite row, 1:1 with
        /// the version (the shipped default). Transactional with the version row
        /// and directly full-text indexable (ADR-018 §3.5 / feature F23), at the
        /// cost of growing the relational backup with body history.
        /// </summary>
        Database = 4,

        /// <summary>
        /// Store the body as an immutable object-store blob (the same machinery
        /// as media, but a body, not a media asset). Keeps body bytes out of the
        /// relational backup for large deployments; not directly full-text
        /// indexable, so search becomes a projection (ADR-018N §2.5).
        /// </summary>
        ObjectStore = 5,

        /// <summary>
        /// Store the body as a file under a configured external content
        /// repository (e.g. a Git working copy) for the Phase-J
        /// documentation-as-source-code round-trip and Git-editable bodies. Not
        /// transactional with the DB; consistency between row and file is
        /// eventual. Never writes into the module source tree.
        /// </summary>
        FileSystem = 6,
    }
}
