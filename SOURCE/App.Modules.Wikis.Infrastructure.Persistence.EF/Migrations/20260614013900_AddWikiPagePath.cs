using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWikiPagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WikiPages_WikiFK_Slug",
                schema: "wikis_wikis",
                table: "WikiPages");

            migrationBuilder.AlterTable(
                name: "WikiPages",
                schema: "wikis_wikis",
                comment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (the canonical ) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints . Addressing is DokuWiki-style: is the source of truth (prefix = namespace, last segment = leaf). is the derived leaf and is an optional, non-authoritative explicit parent link — ancestry and children are derived from the path.",
                oldComment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (slug + optional parent for a tree) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints .");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "The derived last segment (leaf) of , unique within its . Retained for routing and wiki:{key}:{slug} cross-links; it is computed from and is not an independent authority.",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldComment: "The URL-stable slug segment for this page, unique within its . Used in routing and wiki:{key}:{slug} cross-links.")
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentWikiPageFK",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Optional FK to a parent . Non-authoritative for addressing. Ancestry is derived from (split on /); this column is retained only as an optional explicit parent link and is null for a top-level page. A page whose path-parent does not yet exist is valid (DokuWiki-style): navigating there yields a blank page that is an invitation to create it.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Optional FK to a parent , forming the page tree. null for a top-level page directly under the wiki root.");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentVersionId",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Opaque identifier for the related Current Version aggregate.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Opaque identifier for the related Current Version aggregate.")
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                comment: "The canonical, full namespace path of this page within its (e.g. engineering/onboarding/setup), unique within the wiki root. This is the single source of truth for addressing (DokuWiki-style): the prefix up to the last / is the \"namespace\" and the final segment is the leaf. There is no separate namespace entity. is merely the derived last segment of this path. Path is canonical and may itself be non-URL-safe; the render layer is responsible for URL-mangling it for safe links and copy. Full-text/prefix search runs against this field.")
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPages_Path",
                schema: "wikis_wikis",
                table: "WikiPages",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPages_WikiFK_Path",
                schema: "wikis_wikis",
                table: "WikiPages",
                columns: new[] { "WikiFK", "Path" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPages_WikiFK_Slug",
                schema: "wikis_wikis",
                table: "WikiPages",
                columns: new[] { "WikiFK", "Slug" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WikiPages_Path",
                schema: "wikis_wikis",
                table: "WikiPages");

            migrationBuilder.DropIndex(
                name: "IX_WikiPages_WikiFK_Path",
                schema: "wikis_wikis",
                table: "WikiPages");

            migrationBuilder.DropIndex(
                name: "IX_WikiPages_WikiFK_Slug",
                schema: "wikis_wikis",
                table: "WikiPages");

            migrationBuilder.DropColumn(
                name: "Path",
                schema: "wikis_wikis",
                table: "WikiPages");

            migrationBuilder.AlterTable(
                name: "WikiPages",
                schema: "wikis_wikis",
                comment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (slug + optional parent for a tree) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints .",
                oldComment: "A WikiPage is the stable identity of a page within a . It is deliberately thin: it owns the addressing (the canonical ) and points at the current published version, but it carries no body text. All body content is immutable and lives in rows (ADR-018 immutable-blob invariant), so \"editing a page\" never mutates this row's content — it appends a new version and repoints . Addressing is DokuWiki-style: is the source of truth (prefix = namespace, last segment = leaf). is the derived leaf and is an optional, non-authoritative explicit parent link — ancestry and children are derived from the path.");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "The URL-stable slug segment for this page, unique within its . Used in routing and wiki:{key}:{slug} cross-links.",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldComment: "The derived last segment (leaf) of , unique within its . Retained for routing and wiki:{key}:{slug} cross-links; it is computed from and is not an independent authority.")
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentWikiPageFK",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Optional FK to a parent , forming the page tree. null for a top-level page directly under the wiki root.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Optional FK to a parent . Non-authoritative for addressing. Ancestry is derived from (split on /); this column is retained only as an optional explicit parent link and is null for a top-level page. A page whose path-parent does not yet exist is valid (DokuWiki-style): navigating there yields a blank page that is an invitation to create it.");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentVersionId",
                schema: "wikis_wikis",
                table: "WikiPages",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Opaque identifier for the related Current Version aggregate.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Opaque identifier for the related Current Version aggregate.")
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPages_WikiFK_Slug",
                schema: "wikis_wikis",
                table: "WikiPages",
                columns: new[] { "WikiFK", "Slug" },
                unique: true);
        }
    }
}
