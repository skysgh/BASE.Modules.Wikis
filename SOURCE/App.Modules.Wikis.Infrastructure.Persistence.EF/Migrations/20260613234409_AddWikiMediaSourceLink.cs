using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWikiMediaSourceLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceMediaFK",
                schema: "wikis_wikis",
                table: "WikiMedia",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Optional self-referential FK to the source media artifact this row was rendered from, used for the draw.io two-artifact pair (ADR-018, §10 of the body-storage implementation note). On a render artifact (e.g. a flattened image/svg+xml, see ) this points at the editable source artifact (e.g. the application/vnd.jgraph.mxfile, see ) the editor reopens for edit. On a source artifact (or any plain media) this is null. Navigable, so the suffix is FK, not Id. Keeping both artifacts as rows preserves a single ACL surface, a single immutable-blob lifecycle, and a single storage path derivation for the diagram pair.")
                .Annotation("Relational:ColumnOrder", 14);

            migrationBuilder.CreateIndex(
                name: "IX_WikiMedia_SourceMediaFK",
                schema: "wikis_wikis",
                table: "WikiMedia",
                column: "SourceMediaFK");

            migrationBuilder.AddForeignKey(
                name: "FK_WikiMedia_WikiMedia_SourceMediaFK",
                schema: "wikis_wikis",
                table: "WikiMedia",
                column: "SourceMediaFK",
                principalSchema: "wikis_wikis",
                principalTable: "WikiMedia",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WikiMedia_WikiMedia_SourceMediaFK",
                schema: "wikis_wikis",
                table: "WikiMedia");

            migrationBuilder.DropIndex(
                name: "IX_WikiMedia_SourceMediaFK",
                schema: "wikis_wikis",
                table: "WikiMedia");

            migrationBuilder.DropColumn(
                name: "SourceMediaFK",
                schema: "wikis_wikis",
                table: "WikiMedia");
        }
    }
}
