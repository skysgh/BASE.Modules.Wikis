using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWikiNodeStyles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WikiNodeStyles",
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
                    SectionKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "Stable section key (markdown heading slug / rendered heading id) this style applies to. Null or empty means the style decorates the whole page wrapper rather than one specific section."),
                    BackgroundMediaName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "Logical page-vicinity media name (for example, media:hero.png) used as the bounded background asset reference."),
                    OverlayOpacityMode = table.Column<int>(type: "int", nullable: false, comment: "Bounded overlay opacity mode controlling how strongly the background image is muted behind foreground content."),
                    ContrastMode = table.Column<int>(type: "int", nullable: false, comment: "Bounded text contrast mode controlling the foreground treatment over the background image."),
                    WikiPageFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the owning whose rendered output this style decorates.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiNodeStyles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiNodeStyles_WikiPages_WikiPageFK",
                        column: x => x.WikiPageFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Additive page-scoped style row describing a bounded section or page-level background presentation (ADR-018D §2.2, build-plan Phase D step 16).");

            migrationBuilder.CreateIndex(
                name: "IX_WikiNodeStyle_Id",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiNodeStyle_RecordState",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                column: "RecordState");

            migrationBuilder.CreateIndex(
                name: "IX_WikiNodeStyle_WikiPageFK",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                column: "WikiPageFK");

            migrationBuilder.CreateIndex(
                name: "IX_WikiNodeStyles_WikiPageFK_SectionKey",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                columns: new[] { "WikiPageFK", "SectionKey" },
                unique: true,
                filter: "[SectionKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WikiNodeStyles",
                schema: "wikis_wikis");
        }
    }
}
