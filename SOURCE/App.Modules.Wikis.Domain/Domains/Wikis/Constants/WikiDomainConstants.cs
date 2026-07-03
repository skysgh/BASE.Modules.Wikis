namespace App.Modules.Wikis.Domain.Domains.Wikis.Constants
{
    /// <summary>
    /// Fixed, non-configurable domain constants for the Wikis module.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These are the shipped defaults and structural literals of the wiki
    /// domain. Where a value is administrator-overridable, the corresponding
    /// default here is reused as the seed default on
    /// <see cref="Configuration.Implementations.WikiConfigurationObject"/> so the
    /// shipped default and the config default never drift apart.
    /// </para>
    /// </remarks>
    public static class WikiDomainConstants
    {
        /// <summary>
        /// The default name of the index/root document within a folder, with no
        /// file extension. The extension is implied by the content-format parser
        /// (e.g. a markdown parser resolves <c>home</c> to <c>home.md</c>), so
        /// the stored slug stays format-neutral.
        /// </summary>
        public const string DefaultRootDocumentName = "home";

        /// <summary>
        /// The default content-format key applied to new page versions when none
        /// is supplied, per the ADR-018E content-format DSL.
        /// </summary>
        public const string DefaultContentFormatKey = "markdown";

        /// <summary>
        /// The separator used between segments of a wiki page path
        /// (e.g. <c>a/b/c</c>).
        /// </summary>
        public const string PathSegmentSeparator = "/";

        /// <summary>
        /// The separator used in <c>wiki:{key}:{slug}</c> cross-link tokens.
        /// </summary>
        public const string CrossLinkSeparator = ":";

        /// <summary>
        /// The scheme prefix identifying a wiki cross-link token
        /// (<c>wiki:{key}:{slug}</c>).
        /// </summary>
        public const string CrossLinkScheme = "wiki";

        /// <summary>
        /// The container-relative path prefix under which wiki media blobs are
        /// stored in the object store, e.g. <c>wikis/media/{blobId}.{ext}</c>.
        /// <para>
        /// The path is always recomputable from the immutable <c>BlobId</c>
        /// (ADR-018), so it is never persisted as a separate column — the
        /// <c>Guid</c> stays the single canonical identity and the storage path
        /// is derived on demand.
        /// </para>
        /// </summary>
        public const string MediaBlobPathPrefix = "wikis/media";

        /// <summary>
        /// The neutral file extension applied to a media blob path when the
        /// media (MIME) type does not map to a known, explicit extension.
        /// <para>
        /// Deliberately conservative: the extension is for storage-path
        /// readability only; the authoritative content type is the stored
        /// <c>MediaType</c> on the handle, never the path suffix.
        /// </para>
        /// </summary>
        public const string MediaBlobFallbackExtension = "bin";

        /// <summary>
        /// The shipped default for how the draw.io diagram editor application is
        /// sourced, reused as the seed default on
        /// <see cref="Configuration.Implementations.WikiConfigurationObject.DrawioEditorSourceMode"/>.
        /// <para>
        /// <c>Remote</c> by design: the governance-sensitive concern is local
        /// storage of content, not loading the editor from a hosted origin, so
        /// the default avoids forcing every deployment to self-host the editor
        /// assets. Deployments that must keep the editor on-premise switch to
        /// <see cref="Enums.DrawioEditorSourceMode.SelfHosted"/> and repoint
        /// <see cref="DefaultDrawioEditorBaseUrl"/>.
        /// </para>
        /// </summary>
        public const string DefaultDrawioEditorSourceMode = nameof(Enums.DrawioEditorSourceMode.Remote);

        /// <summary>
        /// The shipped default base URL of the draw.io editor application used
        /// when <see cref="DefaultDrawioEditorSourceMode"/> is <c>Remote</c>.
        /// <para>
        /// Points at the canonical draw.io embed host. A self-hosted deployment
        /// repoints this to its in-house editor origin without any other change,
        /// because the editor source and the diagram-content storage location are
        /// independent concerns.
        /// </para>
        /// </summary>
        public const string DefaultDrawioEditorBaseUrl = "https://embed.diagrams.net/";

        /// <summary>
        /// The reference-token scheme identifying an embedded draw.io diagram in
        /// authored wiki content (<c>drawio:{id}</c>).
        /// <para>
        /// Kept format-neutral and stable (the immutable handle id, never a
        /// volatile URL) so authored diagrams survive infra/route moves and the
        /// future Phase J source-tree round-trip with no token migration — the
        /// same discipline as the <c>media:</c> scheme.
        /// </para>
        /// </summary>
        public const string DrawioReferenceScheme = "drawio";

        /// <summary>
        /// The IANA media (MIME) type of a draw.io diagram <em>source</em>
        /// artifact: the native, round-trippable <c>mxfile</c> XML the editor
        /// loads and saves.
        /// <para>
        /// A diagram is persisted as a two-artifact pair (ADR-018, §10 of the
        /// body-storage implementation note): an editable source in this type
        /// plus a flattened render in <see cref="DrawioRenderMediaType"/>. The
        /// render is what authored content displays; the source is what the
        /// editor reopens for edit, resolved by following the render handle's
        /// source link. Storing the native source — rather than a derived form —
        /// keeps diagrams losslessly re-editable across the future Phase J
        /// source-tree round-trip.
        /// </para>
        /// </summary>
        public const string DrawioSourceMediaType = "application/vnd.jgraph.mxfile";

        /// <summary>
        /// The IANA media (MIME) type of a draw.io diagram <em>render</em>
        /// artifact: the flattened, display-ready SVG produced from the
        /// <see cref="DrawioSourceMediaType"/> source.
        /// <para>
        /// SVG is chosen so diagrams stay crisp at any zoom and remain
        /// inline-displayable without the editor present. This is the artifact a
        /// <c>drawio:{id}</c> token (see <see cref="DrawioReferenceScheme"/>)
        /// resolves to for display; the editable source is reached from it via
        /// the render handle's source link.
        /// </para>
        /// </summary>
        public const string DrawioRenderMediaType = "image/svg+xml";

        /// <summary>
        /// The shipped default authoritative body storage sink for new page
        /// versions (ADR-018N). Reused as the seed default on
        /// <see cref="Configuration.Implementations.WikiConfigurationObject.BodyStoragePrimarySink"/>.
        /// <para>
        /// <c>Database</c> by design: it is transactional with the version row,
        /// is directly full-text indexable (ADR-018 §3.5 / feature F23), and is
        /// the simplest, dev-friendly default. Large deployments move bodies out
        /// of the relational backup by switching to the object-store or
        /// file-system sink.
        /// </para>
        /// </summary>
        public const string DefaultBodyStoragePrimarySink = nameof(Enums.WikiBodyStorageSinkKind.Database);

        /// <summary>
        /// The container-relative path prefix under which wiki page-version
        /// <em>body</em> blobs are stored when the object-store body sink is
        /// active, e.g. <c>wikis/bodies/{blobId}.{ext}</c>.
        /// <para>
        /// Deliberately distinct from <see cref="MediaBlobPathPrefix"/> so a
        /// version body and a media asset can never collide in the store, and so
        /// the two lifecycles stay operationally separable. As with media, the
        /// path is recomputed from the immutable body locator and never persisted
        /// as a separate column.
        /// </para>
        /// </summary>
        public const string BodyBlobPathPrefix = "wikis/bodies";

        /// <summary>
        /// The IANA media type a version body blob is stored under by the
        /// object-store body sink. A body is authored text in a declared content
        /// format (ADR-018E); it is stored and served as <c>text/plain</c> while
        /// the authoritative format remains the version's <c>ContentFormatKey</c>,
        /// not this transport media type.
        /// </summary>
        public const string BodyObjectStoreMediaType = "text/plain";

        /// <summary>
        /// The container-relative path prefix under which wiki page-version
        /// bodies are written when the file-system content-repository body sink
        /// is active, relative to the configured external content-repository root
        /// (ADR-018N §2.2). The leaf path is derived from the wiki key, slug, and
        /// version number so a human (or Git tool) browsing the repo sees a
        /// recognisable per-page history.
        /// </summary>
        public const string FileSystemBodyPathPrefix = "wikis";

        /// <summary>
        /// The file extension applied to a file-system body file. Bodies are
        /// text in a declared content format (ADR-018E); the extension is for
        /// repo readability only — the authoritative format is the version's
        /// <c>ContentFormatKey</c>, never the file suffix.
        /// </summary>
        public const string FileSystemBodyFileExtension = "txt";
    }
}
