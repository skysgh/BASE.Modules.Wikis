using App.Modules.Wikis;
using App.Modules.Sys.Shared.Constants;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Constants
{
    /// <summary>
    /// Configuration keys for the Wikis module's admin-tunable behaviour.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All keys are composed from constants (no magic strings) and hang off the
    /// shared services configuration root. These keys back the
    /// <see cref="Configuration.Implementations.WikiConfigurationObject"/> and
    /// let administrators tune resolution and rendering behaviour per
    /// deployment.
    /// </para>
    /// </remarks>
    public static class WikisConfigKeys
    {
        /// <summary>
        /// Wiki behaviour configuration section name.
        /// <para><c>Wiki</c></para>
        /// </summary>
        public const string Name = "Wiki";

        /// <summary>
        /// Configuration root for the module.
        /// <para><c>App:Domains:Wiki</c></para>
        /// </summary>
        public const string Root = $"{ServiceConfigKeys.ServicesRoot}:{Name}";

        /// <summary>
        /// Configuration section containing the core wiki behaviour settings.
        /// <para><c>App:Domains:Wiki</c></para>
        /// </summary>
        public const string Wikis = Root;

        /// <summary>
        /// Configuration key for the default name of the index/root document
        /// within a folder (no file extension; the extension is implied by the
        /// content-format parser).
        /// <para><c>App:Domains:Wiki:DefaultRootDocumentName</c></para>
        /// </summary>
        public const string DefaultRootDocumentName = $"{Wikis}:DefaultRootDocumentName";

        /// <summary>
        /// Configuration key selecting how a path such as <c>a/b/c</c> is
        /// resolved against folder-index documents versus a leaf page.
        /// <para><c>App:Domains:Wiki:FolderIndexResolutionMode</c></para>
        /// </summary>
        public const string FolderIndexResolutionMode = $"{Wikis}:FolderIndexResolutionMode";

        /// <summary>
        /// Configuration key for the default content-format key applied to new
        /// page versions when none is specified (per the ADR-018E format DSL).
        /// <para><c>App:Domains:Wiki:DefaultContentFormatKey</c></para>
        /// </summary>
        public const string DefaultContentFormatKey = $"{Wikis}:DefaultContentFormatKey";

        /// <summary>
        /// Configuration key toggling resolution of <c>wiki:{key}:{slug}</c>
        /// cross-wiki links across mounted wiki roots.
        /// <para><c>App:Domains:Wiki:EnableCrossWikiLinks</c></para>
        /// </summary>
        public const string EnableCrossWikiLinks = $"{Wikis}:EnableCrossWikiLinks";

        /// <summary>
        /// Configuration key toggling whether the slug of an unresolved
        /// cross-link renders as a "create this page" affordance (wiki-style
        /// red links) rather than inert text.
        /// <para><c>App:Domains:Wiki:RenderBrokenLinksAsCreateAffordance</c></para>
        /// </summary>
        public const string RenderBrokenLinksAsCreateAffordance = $"{Wikis}:RenderBrokenLinksAsCreateAffordance";

        /// <summary>
        /// Configuration key toggling whether the draw.io diagram editor is
        /// available at all for authoring. When disabled, existing diagrams still
        /// render but no editing affordance is offered.
        /// <para><c>App:Domains:Wiki:EnableDrawioEditor</c></para>
        /// </summary>
        public const string EnableDrawioEditor = $"{Wikis}:EnableDrawioEditor";

        /// <summary>
        /// Configuration key selecting where the draw.io editor application is
        /// sourced from (<c>Remote</c> hosted origin versus <c>SelfHosted</c>
        /// on-premise origin).
        /// <para><c>App:Domains:Wiki:DrawioEditorSourceMode</c></para>
        /// </summary>
        public const string DrawioEditorSourceMode = $"{Wikis}:DrawioEditorSourceMode";

        /// <summary>
        /// Configuration key for the base URL of the draw.io editor application.
        /// Used as the remote embed origin, or repointed at an in-house deploy
        /// when the source mode is <c>SelfHosted</c>.
        /// <para><c>App:Domains:Wiki:DrawioEditorBaseUrl</c></para>
        /// </summary>
        public const string DrawioEditorBaseUrl = $"{Wikis}:DrawioEditorBaseUrl";

        /// <summary>
        /// Configuration key selecting the authoritative (write + read) body
        /// storage sink for new page versions, per ADR-018N (<c>Database</c> /
        /// <c>ObjectStore</c> / <c>FileSystem</c>).
        /// <para><c>App:Domains:Wiki:BodyStoragePrimarySink</c></para>
        /// </summary>
        public const string BodyStoragePrimarySink = $"{Wikis}:BodyStoragePrimarySink";

        /// <summary>
        /// Configuration key for the optional, ordered set of additional
        /// best-effort body storage sinks a version body is also written to on
        /// save (e.g. mirror the DB-authoritative body to a file content repo so
        /// nothing is lost on a DB wipe), per ADR-018N §2.3.
        /// <para><c>App:Domains:Wiki:BodyStorageMirrorSinks</c></para>
        /// </summary>
        public const string BodyStorageMirrorSinks = $"{Wikis}:BodyStorageMirrorSinks";

        /// <summary>
        /// Configuration key for the external content-repository root path used
        /// by the <c>FileSystem</c> body sink (ADR-018N §2.2). Must be outside
        /// the module source tree; the sink refuses to write when it is unset or
        /// missing.
        /// <para><c>App:Domains:Wiki:BodyStorageFileSystemContentRepositoryRootPath</c></para>
        /// </summary>
        public const string BodyStorageFileSystemContentRepositoryRootPath = $"{Wikis}:BodyStorageFileSystemContentRepositoryRootPath";

        /// <summary>
        /// Configuration key toggling whether a mirror-sink write failure fails
        /// the whole save (<c>true</c>) or is logged best-effort and tolerated
        /// (<c>false</c>, the default), per ADR-018N §2.3.
        /// <para><c>App:Domains:Wiki:BodyStorageFailIfMirrorSinkUnavailable</c></para>
        /// </summary>
        public const string BodyStorageFailIfMirrorSinkUnavailable = $"{Wikis}:BodyStorageFailIfMirrorSinkUnavailable";

        /// <summary>
        /// Configuration key for the default wiki host namespace presented to
        /// the client when no explicit namespace is configured in a wiki leaf
        /// model. An empty string instructs the client to use the root namespace.
        /// <para><c>App:Domains:Wiki:DefaultHostNamespace</c></para>
        /// </summary>
        public const string DefaultHostNamespace = $"{Wikis}:DefaultHostNamespace";

        /// <summary>
        /// Configuration key for the default wiki host slug presented to the
        /// client when no explicit slug is configured in a wiki leaf model. An
        /// empty string instructs the client to substitute the configured
        /// <see cref="DefaultRootDocumentName"/> (e.g. <c>home</c>).
        /// <para><c>App:Domains:Wiki:DefaultHostSlug</c></para>
        /// </summary>
        public const string DefaultHostSlug = $"{Wikis}:DefaultHostSlug";

        /// <summary>
        /// Configuration key for the message rendered in the wiki host when the
        /// requested page does not yet have any content. Shown only after the
        /// server response has been received and confirmed empty — never while
        /// the request is still in flight.
        /// <para><c>App:Domains:Wiki:NoContentMessage</c></para>
        /// </summary>
        public const string NoContentMessage = $"{Wikis}:NoContentMessage";
    }
}
