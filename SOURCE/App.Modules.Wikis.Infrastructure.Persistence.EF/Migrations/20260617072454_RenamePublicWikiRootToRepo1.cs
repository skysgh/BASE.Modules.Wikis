using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePublicWikiRootToRepo1 : Migration
    {
        private static readonly Guid OldWikiId = new Guid("31d732f1-e968-d4af-d5f1-96eb0df3bf98");
        private static readonly Guid NewWikiId = new Guid("c88f756c-e1b6-2d6a-5ed3-61c4964642bd");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "wikis_wikis",
                table: "Wikis",
                columns: new[] { "Id", "CreatedByPrincipalId", "CreatedOnDateTimeUtc", "Description", "Enabled", "Key", "LastModifiedByPrincipalId", "LastModifiedOnDateTimeUtc", "OwnerWorkspaceId", "RecordState", "StateChangedByPrincipalId", "StateChangedOnDateTimeUtc", "Title" },
                values: new object[] { NewWikiId, "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The default wiki document store.", true, "repo1", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Repository 1" });

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiPages] SET [WikiFK] = '{NewWikiId:D}' WHERE [WikiFK] = '{OldWikiId:D}';");

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiTemplates] SET [WikiFK] = '{NewWikiId:D}' WHERE [WikiFK] = '{OldWikiId:D}';");

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiAcls] SET [WikiFK] = '{NewWikiId:D}' WHERE [WikiFK] = '{OldWikiId:D}';");

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiTemplateBindings] SET [WikiId] = '{NewWikiId:D}' WHERE [WikiId] = '{OldWikiId:D}';");

            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: OldWikiId);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "wikis_wikis",
                table: "Wikis",
                columns: new[] { "Id", "CreatedByPrincipalId", "CreatedOnDateTimeUtc", "Description", "Enabled", "Key", "LastModifiedByPrincipalId", "LastModifiedOnDateTimeUtc", "OwnerWorkspaceId", "RecordState", "StateChangedByPrincipalId", "StateChangedOnDateTimeUtc", "Title" },
                values: new object[] { OldWikiId, "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The public, default wiki space.", true, "public", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Public" });

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiPages] SET [WikiFK] = '{OldWikiId:D}' WHERE [WikiFK] = '{NewWikiId:D}';");

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiTemplates] SET [WikiFK] = '{OldWikiId:D}' WHERE [WikiFK] = '{NewWikiId:D}';");

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiAcls] SET [WikiFK] = '{OldWikiId:D}' WHERE [WikiFK] = '{NewWikiId:D}';");

            migrationBuilder.Sql(
                $"UPDATE [wikis_wikis].[WikiTemplateBindings] SET [WikiId] = '{OldWikiId:D}' WHERE [WikiId] = '{NewWikiId:D}';");

            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: NewWikiId);
        }
    }
}
