using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelDrift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiMedia",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiMedia",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnDateTimeUtc",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "CreatedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiMedia",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiMedia",
                newName: "CreatedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "LastModifiedOnDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "CreatedOnDateTimeUtc");
        }
    }
}
