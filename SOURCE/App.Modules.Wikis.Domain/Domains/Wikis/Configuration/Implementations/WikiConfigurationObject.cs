using App.Modules.Wikis;
using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Sys.Shared.Attributes;
using App.Modules.Sys.Shared.Domains.Configuration.Attributes;
using App.Modules.Sys.Shared.Domains.Configuration.Models;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Configuration.Implementations
{
    /// <summary>
    /// Administrator-tunable configuration section governing wiki resolution and
    /// rendering behaviour for the Wikis module.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the production replacement for the template example configuration
    /// object. It exposes the behavioural flags that differ legitimately between
    /// deployments (folder index resolution, default document/format names, and
    /// cross-link handling) without introducing magic strings. The bound section
    /// path is <c>App:Domains:Wiki</c>.
    /// </para>
    /// <para>
    /// Inherits from <see cref="IConfigurationsGroup"/> (which in turn carries the
    /// singleton lifecycle), so it is discovered and bound by convention rather
    /// than manual startup wiring.
    /// </para>
    /// </remarks>
    [Alias(WikisConfigKeys.Wikis)]
    [ConfigurationsGroupDescription(
        SectionPath,
        "Wiki Configuration",
        "Configuration governing wiki resolution, rendering, editor behavior, and body storage defaults.")]
    public class WikiConfigurationObject : IConfigurationsGroup
    {
        /// <summary>
        /// Configuration section path for the wiki behaviour settings.
        /// </summary>
        public const string SectionPath = WikisConfigKeys.Wikis;

        /// <summary>
        /// Gets or sets the default name of the index/root document within a
        /// folder. No file extension is stored: the extension is implied by the
        /// content-format parser (e.g. <c>home</c> resolves to <c>home.md</c>
        /// under a markdown parser).
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.DefaultRootDocumentName,
            "Default Root Document Name",
            "The extension-less slug of the index document looked up inside a folder (for example 'home'). The extension is supplied by the content-format parser.",
            WikiDomainConstants.DefaultRootDocumentName)]
        public string DefaultRootDocumentName { get; set; } = WikiDomainConstants.DefaultRootDocumentName;

        /// <summary>
        /// Gets or sets how a folder-style path such as <c>a/b/c</c> is resolved
        /// against a folder index document versus a leaf page.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.FolderIndexResolutionMode,
            "Folder Index Resolution Mode",
            "Whether 'a/b/c' is resolved as the folder index 'a/b/c/{root}' before falling back to the leaf page 'a/b/c', or the reverse, or only one of them.",
            nameof(Enums.FolderIndexResolutionMode.IndexThenPage))]
        public FolderIndexResolutionMode FolderIndexResolutionMode { get; set; } = FolderIndexResolutionMode.IndexThenPage;

        /// <summary>
        /// Gets or sets the default content-format key applied to new page
        /// versions when none is supplied (per the ADR-018E content-format DSL).
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.DefaultContentFormatKey,
            "Default Content Format Key",
            "The content-format key applied to new page versions when none is specified (for example 'markdown').",
            WikiDomainConstants.DefaultContentFormatKey)]
        public string DefaultContentFormatKey { get; set; } = WikiDomainConstants.DefaultContentFormatKey;

        /// <summary>
        /// Gets or sets a value indicating whether <c>wiki:{key}:{slug}</c>
        /// cross-wiki links are resolved across mounted wiki roots.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.EnableCrossWikiLinks,
            "Enable Cross-Wiki Links",
            "When enabled, 'wiki:{key}:{slug}' tokens resolve to pages in other mounted wiki roots; when disabled they are treated as plain text.",
            "true")]
        public bool EnableCrossWikiLinks { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether an unresolved link renders as
        /// a "create this page" affordance (wiki-style red links) rather than
        /// inert text.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.RenderBrokenLinksAsCreateAffordance,
            "Render Broken Links As Create Affordance",
            "When enabled, a link to a non-existent page renders as a create-page affordance (red link) instead of inert text.",
            "true")]
        public bool RenderBrokenLinksAsCreateAffordance { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the draw.io diagram editor is
        /// available for authoring. When disabled, existing diagrams still render
        /// but no editing affordance is offered.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.EnableDrawioEditor,
            "Enable Draw.io Editor",
            "When enabled, authors can create and edit draw.io diagrams; when disabled, existing diagrams still render but no editing affordance is offered.",
            "true")]
        public bool EnableDrawioEditor { get; set; } = true;

        /// <summary>
        /// Gets or sets where the draw.io editor application is sourced from.
        /// <c>Remote</c> loads the editor from a hosted origin (the shipped
        /// default); <c>SelfHosted</c> loads it from an on-premise origin for
        /// deployments that must keep the editor assets local. This is
        /// independent of where diagram content is persisted.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.DrawioEditorSourceMode,
            "Draw.io Editor Source Mode",
            "Where the draw.io editor application is loaded from: 'Remote' (a hosted origin; the default, because the governance concern is local storage of content, not editor source) or 'SelfHosted' (an on-premise origin).",
            WikiDomainConstants.DefaultDrawioEditorSourceMode)]
        public DrawioEditorSourceMode DrawioEditorSourceMode { get; set; } = DrawioEditorSourceMode.Remote;

        /// <summary>
        /// Gets or sets the base URL of the draw.io editor application. Used as
        /// the remote embed origin, or repointed at an in-house deploy when
        /// <see cref="DrawioEditorSourceMode"/> is <c>SelfHosted</c>.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.DrawioEditorBaseUrl,
            "Draw.io Editor Base URL",
            "The base URL of the draw.io editor application (the remote embed origin, or an in-house origin when the source mode is 'SelfHosted').",
            WikiDomainConstants.DefaultDrawioEditorBaseUrl)]
        public string DrawioEditorBaseUrl { get; set; } = WikiDomainConstants.DefaultDrawioEditorBaseUrl;

        /// <summary>
        /// Gets or sets the authoritative (write + read) storage sink for new
        /// page-version bodies (ADR-018N). <c>Database</c> is the shipped default
        /// (transactional and full-text-indexable); <c>ObjectStore</c> keeps
        /// body bytes out of the relational backup; <c>FileSystem</c> writes
        /// bodies to a configured external content repository for the Phase-J
        /// source-tree round-trip. This is independent of where media assets are
        /// stored (media is always object-store blob-backed).
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.BodyStoragePrimarySink,
            "Body Storage Primary Sink",
            "Where new page-version bodies are authoritatively written and read from: 'Database' (transactional, full-text indexable; the default), 'ObjectStore' (immutable blob, keeps bodies out of the relational backup), or 'FileSystem' (a configured external content repository for the documentation-as-source-code round-trip).",
            WikiDomainConstants.DefaultBodyStoragePrimarySink)]
        public WikiBodyStorageSinkKind BodyStoragePrimarySink { get; set; } = WikiBodyStorageSinkKind.Database;

        /// <summary>
        /// Gets or sets the optional, ordered set of additional best-effort
        /// sinks a page-version body is <em>also</em> written to on save
        /// (ADR-018N §2.3). Mirrors are write-only durability/round-trip targets
        /// (never read), so e.g. a <c>Database</c> primary with a
        /// <c>FileSystem</c> mirror keeps the DB authoritative while round-tripping
        /// every body to a Git-editable content repo. Empty by default.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.BodyStorageMirrorSinks,
            "Body Storage Mirror Sinks",
            "Optional ordered additional sinks a body is also written to on save (best-effort, write-only). For example mirror a 'Database'-authoritative body to 'FileSystem' so nothing is lost if the database is wiped. Empty means no mirroring.",
            null)]
        public IReadOnlyList<WikiBodyStorageSinkKind> BodyStorageMirrorSinks { get; set; } = Array.Empty<WikiBodyStorageSinkKind>();

        /// <summary>
        /// Gets or sets the external content-repository root path used by the
        /// <c>FileSystem</c> body sink (ADR-018N §2.2). Must be an absolute path
        /// <em>outside</em> the module source tree (for example a dedicated wiki
        /// content Git working copy). The file-system sink refuses to write when
        /// this is unset or missing rather than ever writing into application
        /// source; it has no effect for the other sinks.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.BodyStorageFileSystemContentRepositoryRootPath,
            "Body Storage File-System Content Repository Root Path",
            "Absolute root path of the external wiki content repository used by the 'FileSystem' body sink (for example a dedicated wiki content Git working copy). Must be outside the module source tree; the sink fails loudly if it is unset or missing. Ignored by the other sinks.",
            null)]
        public string? BodyStorageFileSystemContentRepositoryRootPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a mirror-sink write failure
        /// fails the whole save (<c>true</c>) or is logged best-effort and
        /// tolerated (<c>false</c>, the default), per ADR-018N §2.3. Best-effort
        /// is the default so a missing content repo never blocks authoring.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.BodyStorageFailIfMirrorSinkUnavailable,
            "Body Storage Fail If Mirror Sink Unavailable",
            "When enabled, a failure writing to any mirror sink fails the whole save; when disabled (the default), mirror failures are logged best-effort and the primary write still succeeds.",
            "false")]
        public bool BodyStorageFailIfMirrorSinkUnavailable { get; set; }

        /// <summary>
        /// Gets or sets the default wiki root namespace key presented to the
        /// client when no explicit namespace is configured in a hosted wiki leaf
        /// model. An empty string instructs the client to address the root
        /// namespace (the key-less or default-keyed wiki root). Defaults to
        /// <see cref="string.Empty"/>.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.DefaultHostNamespace,
            "Default Host Namespace",
            "The default wiki root namespace key presented to the client when no explicit namespace is configured. Empty string means the client should use the root/default namespace.",
            "")]
        public string DefaultHostNamespace { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default page slug presented to the client when no
        /// explicit slug is configured in a hosted wiki leaf model. An empty
        /// string instructs the client to substitute the
        /// <see cref="DefaultRootDocumentName"/> value (e.g. <c>home</c>) so
        /// the translation is driven from a single configurable source.
        /// Defaults to <see cref="string.Empty"/>.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.DefaultHostSlug,
            "Default Host Slug",
            "The default page slug presented to the client when no explicit slug is configured. Empty string instructs the client to use the DefaultRootDocumentName (e.g. 'home') as the effective slug.",
            "")]
        public string DefaultHostSlug { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message rendered inside the wiki host control when
        /// the requested page does not yet have any content. This message is
        /// shown only after the server has responded and confirmed there is no
        /// content — it is never shown while the request is still in flight.
        /// </summary>
        [ConfigurationPropertyDescriptionAttribute(
            true,
            true,
            WikisConfigKeys.NoContentMessage,
            "No Content Message",
            """
            # No Content
                    
            Create a new page by selecting *Edit* 
            and developing a page with an H1 title and some content. 
            """)]
        public string NoContentMessage { get; set; } = "# No Content #\n\nCreate a new page by selecting *Edit*\nand developing a page with an H1 title and some content.";
    }
}
