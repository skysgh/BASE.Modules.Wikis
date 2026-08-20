using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FoldWikiAclOptionalReferenceFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_WikiAcls_WikiPageFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcl_WikiPageFK");

            migrationBuilder.RenameIndex(
                name: "IX_WikiAcls_WikiFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcl_WikiFK");

            migrationBuilder.AlterColumn<Guid>(
                name: "WikiPageFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                type: "uniqueidentifier",
                nullable: true,
                comment: "FK to the this grant applies to, when the grant is page-scoped. null when the grant is wiki-wide.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "FK to the this grant applies to, when the grant is page-scoped. null when the grant is wiki-wide.")
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<Guid>(
                name: "WikiFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                type: "uniqueidentifier",
                nullable: true,
                comment: "FK to the root this grant applies to, when the grant is wiki-wide. null when the grant is page-scoped.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "FK to the root this grant applies to, when the grant is wiki-wide. null when the grant is page-scoped.")
                .Annotation("Relational:ColumnOrder", 12);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_WikiAcl_WikiPageFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcls_WikiPageFK");

            migrationBuilder.RenameIndex(
                name: "IX_WikiAcl_WikiFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcls_WikiFK");

            migrationBuilder.AlterColumn<Guid>(
                name: "WikiPageFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                type: "uniqueidentifier",
                nullable: true,
                comment: "FK to the this grant applies to, when the grant is page-scoped. null when the grant is wiki-wide.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "FK to the this grant applies to, when the grant is page-scoped. null when the grant is wiki-wide.")
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<Guid>(
                name: "WikiFK",
                schema: "wikis_wikis",
                table: "WikiAcls",
                type: "uniqueidentifier",
                nullable: true,
                comment: "FK to the root this grant applies to, when the grant is wiki-wide. null when the grant is page-scoped.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "FK to the root this grant applies to, when the grant is wiki-wide. null when the grant is page-scoped.")
                .OldAnnotation("Relational:ColumnOrder", 12);
        }
    }
}
