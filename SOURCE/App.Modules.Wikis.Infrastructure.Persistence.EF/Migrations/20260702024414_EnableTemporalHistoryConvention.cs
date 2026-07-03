using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnableTemporalHistoryConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "WikiTemplateSections",
                schema: "wikis_wikis",
                comment: "A single ordered block of a scaffold (ADR-018C, build-plan Phase D step 15): a heading plus authoring guidance, and the structural expectation a page is later linted against. The order here is determinate, not cosmetic: it fixes the sequence sections are emitted into a scaffolded page and the sequence the structural lint walks them, so it implements (logic ordering) rather than (display hint). See the DisplayOrderHint/PrecedenceOrder distinction in the house rules.",
                oldComment: "A single ordered block of a scaffold (ADR-018C, build-plan Phase D step 15): a heading plus authoring guidance, and the structural expectation a page is later linted against. The order here is determinate, not cosmetic: it fixes the sequence sections are emitted into a scaffolded page and the sequence the structural lint walks them, so it implements (logic ordering) rather than (display hint). See the DisplayOrderHint/PrecedenceOrder distinction in the house rules.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiTemplateSectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiTemplates",
                schema: "wikis_wikis",
                comment: "A WikiTemplate is a reusable page scaffold (ADR-018C, build-plan Phase D step 15). It defines the shape a new page should start from — an ordered set of blocks (headings + authoring guidance) — and the structural expectations a page is later linted against (advisory by default, never blocking). Templates are themselves authored as pages (\"templates-as-pages\") under the conventional _templates/ namespace, so the same versioning, ACL, and rendering machinery applies to a template as to any page. This row is the structured, queryable projection of that template: the body prose lives in a blob like any page, while the section contract and binding live here so scaffolding and lint can reason about structure without re-parsing prose.",
                oldComment: "A WikiTemplate is a reusable page scaffold (ADR-018C, build-plan Phase D step 15). It defines the shape a new page should start from — an ordered set of blocks (headings + authoring guidance) — and the structural expectations a page is later linted against (advisory by default, never blocking). Templates are themselves authored as pages (\"templates-as-pages\") under the conventional _templates/ namespace, so the same versioning, ACL, and rendering machinery applies to a template as to any page. This row is the structured, queryable projection of that template: the body prose lives in a blob like any page, while the section contract and binding live here so scaffolding and lint can reason about structure without re-parsing prose.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiTemplatesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiTemplateBindings",
                schema: "wikis_wikis",
                comment: "Attaches a to a part of a wiki so new pages there default to that template, and existing pages there are linted against it (ADR-018C, build-plan Phase D step 15). A binding targets either a whole namespace (by slug prefix) or a specific page subtree (by ). When more than one binding could apply, decides which wins — this is logic ordering (which template governs), not display, so it uses .",
                oldComment: "Attaches a to a part of a wiki so new pages there default to that template, and existing pages there are linted against it (ADR-018C, build-plan Phase D step 15). A binding targets either a whole namespace (by slug prefix) or a specific page subtree (by ). When more than one binding could apply, decides which wins — this is logic ordering (which template governs), not display, so it uses .")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiTemplateBindingsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "Wikis",
                schema: "wikis_wikis",
                comment: "A Wiki is a single, independently mountable wiki root (a \"space\"). It is the top of the page tree: every belongs to exactly one Wiki, and the is the stable mount key (the namespace segment used in wiki:{key}:{slug} cross-links and in routing). Multiple wikis can co-exist on the platform (per ADR-018H multi-root), so domain-neutral key + title + description is all that is modelled here; everything richer hangs off the pages.",
                oldComment: "A Wiki is a single, independently mountable wiki root (a \"space\"). It is the top of the page tree: every belongs to exactly one Wiki, and the is the stable mount key (the namespace segment used in wiki:{key}:{slug} cross-links and in routing). Multiple wikis can co-exist on the platform (per ADR-018H multi-root), so domain-neutral key + title + description is all that is modelled here; everything richer hangs off the pages.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikisHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiPageVersions",
                schema: "wikis_wikis",
                comment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; is the sink-agnostic body locator (ADR-018N §2.6) addressing the version's raw body bytes in whichever body-storage sink is configured — a WikiPageVersionBody satellite row (Database sink), an immutable object-store blob (ObjectStore sink), or an external content-repo file (FileSystem sink). is the sink-independent content hash of those bytes and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\" regardless of where the bytes physically live.",
                oldComment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; is the sink-agnostic body locator (ADR-018N §2.6) addressing the version's raw body bytes in whichever body-storage sink is configured — a WikiPageVersionBody satellite row (Database sink), an immutable object-store blob (ObjectStore sink), or an external content-repo file (FileSystem sink). is the sink-independent content hash of those bytes and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\" regardless of where the bytes physically live.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiPageVersionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiPageVersionBodies",
                schema: "wikis_wikis",
                comment: "The bytes-bearing satellite row for the Database body storage sink (ADR-018N §2.2, Seam 1). It holds the raw text body of exactly one , 1:1, via a foreign key into the version — it never alters the core row, in keeping with the ADR-018 §2.7 additive-tables seam.",
                oldComment: "The bytes-bearing satellite row for the Database body storage sink (ADR-018N §2.2, Seam 1). It holds the raw text body of exactly one , 1:1, via a foreign key into the version — it never alters the core row, in keeping with the ADR-018 §2.7 additive-tables seam.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiPageVersionBodiesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiPages",
                schema: "wikis_wikis",
                comment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (the canonical ) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints . Addressing is DokuWiki-style: is the source of truth (prefix = namespace, last segment = leaf). is the derived leaf and is an optional, non-authoritative explicit parent link — ancestry and children are derived from the path.",
                oldComment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (the canonical ) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints . Addressing is DokuWiki-style: is the source of truth (prefix = namespace, last segment = leaf). is the derived leaf and is an optional, non-authoritative explicit parent link — ancestry and children are derived from the path.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiPagesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiNodeStyles",
                schema: "wikis_wikis",
                comment: "Additive page-scoped style row describing a bounded section or page-level background presentation (ADR-018D §2.2, build-plan Phase D step 16).",
                oldComment: "Additive page-scoped style row describing a bounded section or page-level background presentation (ADR-018D §2.2, build-plan Phase D step 16).")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiNodeStylesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiMedia",
                schema: "wikis_wikis",
                comment: "A media asset that lives in the vicinity of a (images, diagrams, baked badge images, draw.io SVG, etc.). Like page bodies, media is an immutable blob (ADR-018): \"replace\" means a new and a repoint, never an in-place mutation. This row is the addressable, ACL-able handle for the blob; the bytes themselves are held by the object store via Sys.Infrastructure.Media.",
                oldComment: "A media asset that lives in the vicinity of a (images, diagrams, baked badge images, draw.io SVG, etc.). Like page bodies, media is an immutable blob (ADR-018): \"replace\" means a new and a repoint, never an in-place mutation. This row is the addressable, ACL-able handle for the blob; the bytes themselves are held by the object store via Sys.Infrastructure.Media.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiMediaHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiAcls",
                schema: "wikis_wikis",
                comment: "An access-control entry binding a principal to a permission on a wiki scope, following the framework's share-based access pattern (a TenantId-per-row model is an explicit anti-pattern here). Access is granted by issuing WikiAcl rows to principals rather than by owning a tenant. The / pair identifies who the grant is for (User, Group, Workspace, or Everyone). Exactly one scope FK is populated: an ACL applies either to a whole () or to a single (); the page-level grant is the more specific override. Every resolver applies the reader's grants and must never act as an existence/content oracle for content the reader cannot see.",
                oldComment: "An access-control entry binding a principal to a permission on a wiki scope, following the framework's share-based access pattern (a TenantId-per-row model is an explicit anti-pattern here). Access is granted by issuing WikiAcl rows to principals rather than by owning a tenant. The / pair identifies who the grant is for (User, Group, Workspace, or Everyone). Exactly one scope FK is populated: an ACL applies either to a whole () or to a single (); the page-level grant is the more specific override. Every resolver applies the reader's grants and must never act as an existence/content oracle for content the reader cannot see.")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WikiAclsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Template Section record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Template Section record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Template record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Template record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Template Binding record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Template Binding record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "Wikis",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "Wikis",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Page Version record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Page Version record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Page Version Body record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Page Version Body record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Page record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Page record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Node Style record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Node Style record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiMedia",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Media record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiMedia",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Media record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiAcls",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys End Time value for the Wiki Acl record.")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiAcls",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Stores the Sys Start Time value for the Wiki Acl record.")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiTemplateSections")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiTemplateSections")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiTemplates")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiTemplates")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "Wikis")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "Wikis")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiPageVersions")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiPageVersions")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiPages")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiPages")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiNodeStyles")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiNodeStyles")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiMedia")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiMedia")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                schema: "wikis_wikis",
                table: "WikiAcls")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                schema: "wikis_wikis",
                table: "WikiAcls")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AlterTable(
                name: "WikiTemplateSections",
                schema: "wikis_wikis",
                comment: "A single ordered block of a scaffold (ADR-018C, build-plan Phase D step 15): a heading plus authoring guidance, and the structural expectation a page is later linted against. The order here is determinate, not cosmetic: it fixes the sequence sections are emitted into a scaffolded page and the sequence the structural lint walks them, so it implements (logic ordering) rather than (display hint). See the DisplayOrderHint/PrecedenceOrder distinction in the house rules.",
                oldComment: "A single ordered block of a scaffold (ADR-018C, build-plan Phase D step 15): a heading plus authoring guidance, and the structural expectation a page is later linted against. The order here is determinate, not cosmetic: it fixes the sequence sections are emitted into a scaffolded page and the sequence the structural lint walks them, so it implements (logic ordering) rather than (display hint). See the DisplayOrderHint/PrecedenceOrder distinction in the house rules.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiTemplateSectionsHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiTemplates",
                schema: "wikis_wikis",
                comment: "A WikiTemplate is a reusable page scaffold (ADR-018C, build-plan Phase D step 15). It defines the shape a new page should start from — an ordered set of blocks (headings + authoring guidance) — and the structural expectations a page is later linted against (advisory by default, never blocking). Templates are themselves authored as pages (\"templates-as-pages\") under the conventional _templates/ namespace, so the same versioning, ACL, and rendering machinery applies to a template as to any page. This row is the structured, queryable projection of that template: the body prose lives in a blob like any page, while the section contract and binding live here so scaffolding and lint can reason about structure without re-parsing prose.",
                oldComment: "A WikiTemplate is a reusable page scaffold (ADR-018C, build-plan Phase D step 15). It defines the shape a new page should start from — an ordered set of blocks (headings + authoring guidance) — and the structural expectations a page is later linted against (advisory by default, never blocking). Templates are themselves authored as pages (\"templates-as-pages\") under the conventional _templates/ namespace, so the same versioning, ACL, and rendering machinery applies to a template as to any page. This row is the structured, queryable projection of that template: the body prose lives in a blob like any page, while the section contract and binding live here so scaffolding and lint can reason about structure without re-parsing prose.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiTemplatesHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiTemplateBindings",
                schema: "wikis_wikis",
                comment: "Attaches a to a part of a wiki so new pages there default to that template, and existing pages there are linted against it (ADR-018C, build-plan Phase D step 15). A binding targets either a whole namespace (by slug prefix) or a specific page subtree (by ). When more than one binding could apply, decides which wins — this is logic ordering (which template governs), not display, so it uses .",
                oldComment: "Attaches a to a part of a wiki so new pages there default to that template, and existing pages there are linted against it (ADR-018C, build-plan Phase D step 15). A binding targets either a whole namespace (by slug prefix) or a specific page subtree (by ). When more than one binding could apply, decides which wins — this is logic ordering (which template governs), not display, so it uses .")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiTemplateBindingsHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "Wikis",
                schema: "wikis_wikis",
                comment: "A Wiki is a single, independently mountable wiki root (a \"space\"). It is the top of the page tree: every belongs to exactly one Wiki, and the is the stable mount key (the namespace segment used in wiki:{key}:{slug} cross-links and in routing). Multiple wikis can co-exist on the platform (per ADR-018H multi-root), so domain-neutral key + title + description is all that is modelled here; everything richer hangs off the pages.",
                oldComment: "A Wiki is a single, independently mountable wiki root (a \"space\"). It is the top of the page tree: every belongs to exactly one Wiki, and the is the stable mount key (the namespace segment used in wiki:{key}:{slug} cross-links and in routing). Multiple wikis can co-exist on the platform (per ADR-018H multi-root), so domain-neutral key + title + description is all that is modelled here; everything richer hangs off the pages.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikisHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiPageVersions",
                schema: "wikis_wikis",
                comment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; is the sink-agnostic body locator (ADR-018N §2.6) addressing the version's raw body bytes in whichever body-storage sink is configured — a WikiPageVersionBody satellite row (Database sink), an immutable object-store blob (ObjectStore sink), or an external content-repo file (FileSystem sink). is the sink-independent content hash of those bytes and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\" regardless of where the bytes physically live.",
                oldComment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; is the sink-agnostic body locator (ADR-018N §2.6) addressing the version's raw body bytes in whichever body-storage sink is configured — a WikiPageVersionBody satellite row (Database sink), an immutable object-store blob (ObjectStore sink), or an external content-repo file (FileSystem sink). is the sink-independent content hash of those bytes and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\" regardless of where the bytes physically live.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiPageVersionsHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiPageVersionBodies",
                schema: "wikis_wikis",
                comment: "The bytes-bearing satellite row for the Database body storage sink (ADR-018N §2.2, Seam 1). It holds the raw text body of exactly one , 1:1, via a foreign key into the version — it never alters the core row, in keeping with the ADR-018 §2.7 additive-tables seam.",
                oldComment: "The bytes-bearing satellite row for the Database body storage sink (ADR-018N §2.2, Seam 1). It holds the raw text body of exactly one , 1:1, via a foreign key into the version — it never alters the core row, in keeping with the ADR-018 §2.7 additive-tables seam.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiPageVersionBodiesHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiPages",
                schema: "wikis_wikis",
                comment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (the canonical ) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints . Addressing is DokuWiki-style: is the source of truth (prefix = namespace, last segment = leaf). is the derived leaf and is an optional, non-authoritative explicit parent link — ancestry and children are derived from the path.",
                oldComment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (the canonical ) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints . Addressing is DokuWiki-style: is the source of truth (prefix = namespace, last segment = leaf). is the derived leaf and is an optional, non-authoritative explicit parent link — ancestry and children are derived from the path.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiPagesHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiNodeStyles",
                schema: "wikis_wikis",
                comment: "Additive page-scoped style row describing a bounded section or page-level background presentation (ADR-018D §2.2, build-plan Phase D step 16).",
                oldComment: "Additive page-scoped style row describing a bounded section or page-level background presentation (ADR-018D §2.2, build-plan Phase D step 16).")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiNodeStylesHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiMedia",
                schema: "wikis_wikis",
                comment: "A media asset that lives in the vicinity of a (images, diagrams, baked badge images, draw.io SVG, etc.). Like page bodies, media is an immutable blob (ADR-018): \"replace\" means a new and a repoint, never an in-place mutation. This row is the addressable, ACL-able handle for the blob; the bytes themselves are held by the object store via Sys.Infrastructure.Media.",
                oldComment: "A media asset that lives in the vicinity of a (images, diagrams, baked badge images, draw.io SVG, etc.). Like page bodies, media is an immutable blob (ADR-018): \"replace\" means a new and a repoint, never an in-place mutation. This row is the addressable, ACL-able handle for the blob; the bytes themselves are held by the object store via Sys.Infrastructure.Media.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiMediaHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "WikiAcls",
                schema: "wikis_wikis",
                comment: "An access-control entry binding a principal to a permission on a wiki scope, following the framework's share-based access pattern (a TenantId-per-row model is an explicit anti-pattern here). Access is granted by issuing WikiAcl rows to principals rather than by owning a tenant. The / pair identifies who the grant is for (User, Group, Workspace, or Everyone). Exactly one scope FK is populated: an ACL applies either to a whole () or to a single (); the page-level grant is the more specific override. Every resolver applies the reader's grants and must never act as an existence/content oracle for content the reader cannot see.",
                oldComment: "An access-control entry binding a principal to a permission on a wiki scope, following the framework's share-based access pattern (a TenantId-per-row model is an explicit anti-pattern here). Access is granted by issuing WikiAcl rows to principals rather than by owning a tenant. The / pair identifies who the grant is for (User, Group, Workspace, or Everyone). Exactly one scope FK is populated: an ACL applies either to a whole () or to a single (); the page-level grant is the more specific override. Every resolver applies the reader's grants and must never act as an existence/content oracle for content the reader cannot see.")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "WikiAclsHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "wikis_wikis")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");
        }
    }
}
