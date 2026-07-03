using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wikis_wikis");

            migrationBuilder.CreateTable(
                name: "Wikis",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    Key = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Get/Set the list item's unique key."),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "The (display) title."),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "The textual Description."),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Get/Set whether the entity is enabled or not."),
                    OwnerWorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Opaque identifier for the related Owner Workspace aggregate.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wikis", x => x.Id);
                },
                comment: "A Wiki is a single, independently mountable wiki root (a \"space\"). It is the top of the page tree: every belongs to exactly one Wiki, and the is the stable mount key (the namespace segment used in wiki:{key}:{slug} cross-links and in routing). Multiple wikis can co-exist on the platform (per ADR-018H multi-root), so domain-neutral key + title + description is all that is modelled here; everything richer hangs off the pages.");

            migrationBuilder.CreateTable(
                name: "WikiPages",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "The (display) title."),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "The textual Description."),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Get/Set whether the entity is enabled or not."),
                    WikiFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the owning root. Navigable, so the suffix is FK, not Id."),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The URL-stable slug segment for this page, unique within its . Used in routing and wiki:{key}:{slug} cross-links."),
                    CurrentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Opaque identifier for the related Current Version aggregate."),
                    ParentWikiPageFK = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Optional FK to a parent , forming the page tree. null for a top-level page directly under the wiki root.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiPages_WikiPages_ParentWikiPageFK",
                        column: x => x.ParentWikiPageFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WikiPages_Wikis_WikiFK",
                        column: x => x.WikiFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "Wikis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (slug + optional parent for a tree) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints .");

            migrationBuilder.CreateTable(
                name: "WikiTemplates",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    Key = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Get/Set the list item's unique key."),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "The (display) title."),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "The textual Description."),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Get/Set whether the entity is enabled or not."),
                    ContentFormatKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The declared content format the template scaffolds in (per the ADR-018E content-format DSL, e.g. markdown). New pages created from this template start in this format so the editor emits a declared format from the outset."),
                    WikiFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the owning root the template belongs to. Navigable, so the suffix is FK, not Id.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiTemplates_Wikis_WikiFK",
                        column: x => x.WikiFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "Wikis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "A WikiTemplate is a reusable page scaffold (ADR-018C, build-plan Phase D step 15). It defines the shape a new page should start from — an ordered set of blocks (headings + authoring guidance) — and the structural expectations a page is later linted against (advisory by default, never blocking). Templates are themselves authored as pages (\"templates-as-pages\") under the conventional _templates/ namespace, so the same versioning, ACL, and rendering machinery applies to a template as to any page. This row is the structured, queryable projection of that template: the body prose lives in a blob like any page, while the section contract and binding live here so scaffolding and lint can reason about structure without re-parsing prose.");

            migrationBuilder.CreateTable(
                name: "WikiAcls",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The identifier of the principal the grant is issued to. Interpreted together with ."),
                    PrincipalType = table.Column<int>(type: "int", nullable: false, comment: "The kind of principal (User, Group, Workspace, or Everyone), stored as the integer of the shared PrincipalType contract to avoid a hard enum dependency leaking into this Shared model."),
                    PermissionKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The permission key granted, e.g. Wiki:Read or WikiPage:Write, composed from permission constants rather than a magic string at the call site."),
                    WikiFK = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "FK to the root this grant applies to, when the grant is wiki-wide. null when the grant is page-scoped."),
                    WikiPageFK = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "FK to the this grant applies to, when the grant is page-scoped. null when the grant is wiki-wide.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiAcls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiAcls_WikiPages_WikiPageFK",
                        column: x => x.WikiPageFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WikiAcls_Wikis_WikiFK",
                        column: x => x.WikiFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "Wikis",
                        principalColumn: "Id");
                },
                comment: "An access-control entry binding a principal to a permission on a wiki scope, following the framework's share-based access pattern (a TenantId-per-row model is an explicit anti-pattern here). Access is granted by issuing WikiAcl rows to principals rather than by owning a tenant. The / pair identifies who the grant is for (User, Group, Workspace, or Everyone). Exactly one scope FK is populated: an ACL applies either to a whole () or to a single (); the page-level grant is the more specific override. Every resolver applies the reader's grants and must never act as an existence/content oracle for content the reader cannot see.");

            migrationBuilder.CreateTable(
                name: "WikiMedia",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "The (display) title."),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "The textual Description."),
                    BlobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Identifier of the immutable object-store blob holding the media bytes."),
                    MediaType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The IANA media (MIME) type of the blob, e.g. image/png or image/svg+xml."),
                    ContentHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The content hash of the media blob, used for drift detection and to support re-verification of baked endorsement badges (ADR-018M)."),
                    WikiPageFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the owning this media is attached to. Navigable, so the suffix is FK, not Id.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiMedia_WikiPages_WikiPageFK",
                        column: x => x.WikiPageFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "A media asset that lives in the vicinity of a (images, diagrams, baked badge images, draw.io SVG, etc.). Like page bodies, media is an immutable blob (ADR-018): \"replace\" means a new and a repoint, never an in-place mutation. This row is the addressable, ACL-able handle for the blob; the bytes themselves are held by the object store via Sys.Infrastructure.Media.");

            migrationBuilder.CreateTable(
                name: "WikiPageVersions",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    WikiPageFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the owning . Navigable, so the suffix is FK, not Id."),
                    VersionNumber = table.Column<int>(type: "int", nullable: false, comment: "Monotonic version number within the page (1-based). Combined with it uniquely identifies a revision."),
                    BodyBlobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Identifier of the immutable object-store blob holding this version's raw body bytes. Replacing content means a new blob id, never a mutation of the existing blob."),
                    ContentHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The content hash of the body blob. Used for staleness/drift detection and as the subject a verifiable endorsement (ADR-018M) is pinned to."),
                    ContentFormatKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The declared content format of the body (e.g. markdown), per the ADR-018E content-format DSL. Stored as a key so the parser-selection seam can resolve the right parser without a hard enum dependency at this layer.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiPageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiPageVersions_WikiPages_WikiPageFK",
                        column: x => x.WikiPageFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; points at an immutable object-store blob (reusing Sys.Infrastructure.Media). is the content hash of that blob and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\".");

            migrationBuilder.CreateTable(
                name: "WikiTemplateBindings",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Get/Set whether the entity is enabled or not."),
                    PrecedenceOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "The deterministic evaluation order for this entity. Lower values are evaluated first. Must be unique within the evaluation context to avoid ambiguous resolution."),
                    WikiTemplateFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the being bound. Navigable, so the suffix is FK, not Id."),
                    WikiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Id of the owning root the binding lives in. Deliberately a non-navigable aggregate id (no Wiki navigation): the binding already cascades from its (which is itself owned by the wiki), so adding a second direct Wiki -> Binding cascade would create multiple cascade paths that SQL Server rejects. This scalar is kept purely as a denormalised scope/query key, mirroring the WikiPage.CurrentVersionId \"plain id to avoid a cycle\" precedent."),
                    ScopeSlugPrefix = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "Optional slug prefix this binding scopes to (e.g. how-to/). Empty when the binding scopes by , or to bind the whole wiki."),
                    ScopeWikiPageFK = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Optional FK to a whose subtree this binding scopes to. null means the binding scopes by instead.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiTemplateBindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiTemplateBindings_WikiPages_ScopeWikiPageFK",
                        column: x => x.ScopeWikiPageFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WikiTemplateBindings_WikiTemplates_WikiTemplateFK",
                        column: x => x.WikiTemplateFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Attaches a to a part of a wiki so new pages there default to that template, and existing pages there are linted against it (ADR-018C, build-plan Phase D step 15). A binding targets either a whole namespace (by slug prefix) or a specific page subtree (by ). When more than one binding could apply, decides which wins — this is logic ordering (which template governs), not display, so it uses .");

            migrationBuilder.CreateTable(
                name: "WikiTemplateSections",
                schema: "wikis_wikis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Gets or sets the identifier."),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Gets or sets the datastore concurrency check timestamp."),
                    RecordState = table.Column<int>(type: "int", nullable: false, comment: "The state of the Record in terms of persistence."),
                    CreatedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime created on."),
                    CreatedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who created the record."),
                    LastModifiedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Gets or sets the UTC DateTime when the record was last modified."),
                    LastModifiedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, comment: "Gets or sets the principal id who last modified the record."),
                    StateChangedOnDateTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "Gets or sets the date when record state changed (nullable for soft delete)."),
                    StateChangedByPrincipalId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, comment: "Gets or sets the principal id who changed the state (nullable)."),
                    Key = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Get/Set the list item's unique key."),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "The (display) title."),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "The textual Description."),
                    PrecedenceOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "The deterministic evaluation order for this entity. Lower values are evaluated first. Must be unique within the evaluation context to avoid ambiguous resolution."),
                    WikiTemplateFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the owning . Navigable, so the suffix is FK, not Id."),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, comment: "Whether the structural lint treats this section as required. When true, a page missing this section's heading raises an advisory lint finding (advisory-by-default — it never blocks saving)."),
                    PlaceholderBody = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "Optional placeholder/guidance prose emitted under the heading when a page is scaffolded from the template. Plain authoring text in the template's declared content format; never executable.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiTemplateSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiTemplateSections_WikiTemplates_WikiTemplateFK",
                        column: x => x.WikiTemplateFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "A single ordered block of a scaffold (ADR-018C, build-plan Phase D step 15): a heading plus authoring guidance, and the structural expectation a page is later linted against. The order here is determinate, not cosmetic: it fixes the sequence sections are emitted into a scaffolded page and the sequence the structural lint walks them, so it implements (logic ordering) rather than (display hint). See the DisplayOrderHint/PrecedenceOrder distinction in the house rules.");

            migrationBuilder.CreateIndex(
                name: "IX_WikiAcl_Id",
                schema: "wikis_wikis",
                table: "WikiAcls",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiAcl_RecordState",
                schema: "wikis_wikis",
                table: "WikiAcls",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiAcls_PrincipalId_PrincipalType",
                schema: "wikis_wikis",
                table: "WikiAcls",
                columns: new[] { "PrincipalId", "PrincipalType" });

            migrationBuilder.CreateIndex(
                name: "IX_WikiAcls_WikiFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                column: "WikiFK");

            migrationBuilder.CreateIndex(
                name: "IX_WikiAcls_WikiPageFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                column: "WikiPageFK");

            migrationBuilder.CreateIndex(
                name: "IX_WikiMedia_Id",
                schema: "wikis_wikis",
                table: "WikiMedia",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiMedia_RecordState",
                schema: "wikis_wikis",
                table: "WikiMedia",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiMedia_WikiPageFK",
                schema: "wikis_wikis",
                table: "WikiMedia",
                column: "WikiPageFK");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPage_CurrentVersionId",
                schema: "wikis_wikis",
                table: "WikiPages",
                column: "CurrentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPage_Enabled",
                schema: "wikis_wikis",
                table: "WikiPages",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPage_Id",
                schema: "wikis_wikis",
                table: "WikiPages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPage_RecordState",
                schema: "wikis_wikis",
                table: "WikiPages",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPages_ParentWikiPageFK",
                schema: "wikis_wikis",
                table: "WikiPages",
                column: "ParentWikiPageFK");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPages_WikiFK_Slug",
                schema: "wikis_wikis",
                table: "WikiPages",
                columns: new[] { "WikiFK", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPageVersion_Id",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPageVersion_RecordState",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPageVersions_WikiPageFK_VersionNumber",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                columns: new[] { "WikiPageFK", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wiki_Enabled",
                schema: "wikis_wikis",
                table: "Wikis",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_Wiki_Id",
                schema: "wikis_wikis",
                table: "Wikis",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wiki_OwnerWorkspaceId",
                schema: "wikis_wikis",
                table: "Wikis",
                column: "OwnerWorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Wiki_RecordState",
                schema: "wikis_wikis",
                table: "Wikis",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_Wikis_Key",
                schema: "wikis_wikis",
                table: "Wikis",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateBinding_Enabled",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateBinding_Id",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateBinding_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateBinding_ScopeWikiPageFK",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                column: "ScopeWikiPageFK");

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateBindings_WikiId_PrecedenceOrder",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                columns: new[] { "WikiId", "PrecedenceOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateBindings_WikiTemplateFK",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                column: "WikiTemplateFK");

            migrationBuilder.CreateIndex(
                name: "UX_WikiTemplateBinding_PrecedenceOrder",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                column: "PrecedenceOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplate_Enabled",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplate_Id",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplate_Key",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplate_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplate_WikiFK",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                column: "WikiFK");

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplates_WikiFK_Key",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                columns: new[] { "WikiFK", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateSection_Id",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateSection_Key",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateSection_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiTemplateSections_WikiTemplateFK_Key",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                columns: new[] { "WikiTemplateFK", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_WikiTemplateSection_PrecedenceOrder",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                column: "PrecedenceOrder",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WikiAcls",
                schema: "wikis_wikis");

            migrationBuilder.DropTable(
                name: "WikiMedia",
                schema: "wikis_wikis");

            migrationBuilder.DropTable(
                name: "WikiPageVersions",
                schema: "wikis_wikis");

            migrationBuilder.DropTable(
                name: "WikiTemplateBindings",
                schema: "wikis_wikis");

            migrationBuilder.DropTable(
                name: "WikiTemplateSections",
                schema: "wikis_wikis");

            migrationBuilder.DropTable(
                name: "WikiPages",
                schema: "wikis_wikis");

            migrationBuilder.DropTable(
                name: "WikiTemplates",
                schema: "wikis_wikis");

            migrationBuilder.DropTable(
                name: "Wikis",
                schema: "wikis_wikis");
        }
    }
}
