using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedWikiRoots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "wikis_wikis",
                table: "Wikis",
                columns: new[] { "Id", "CreatedByPrincipalId", "CreatedOnDateTimeUtc", "Description", "Enabled", "Key", "LastModifiedByPrincipalId", "LastModifiedOnDateTimeUtc", "OwnerWorkspaceId", "RecordState", "StateChangedByPrincipalId", "StateChangedOnDateTimeUtc", "Title" },
                values: new object[,]
                {
                    { new Guid("07db6a3f-bde1-d772-d162-36a69635611e"), "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Reference material and resource documentation.", true, "resources", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Resources" },
                    { new Guid("31d732f1-e968-d4af-d5f1-96eb0df3bf98"), "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The public, default wiki space.", true, "public", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Public" },
                    { new Guid("459b165f-0a68-e67f-37cb-0b8720e28494"), "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Developer documentation, spikes, and engineering notes.", true, "developers", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Developers" },
                    { new Guid("46d654b0-5a0b-45d4-6e20-3cc93ea72d2d"), "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Internal, organisation-facing wiki space.", true, "intranet", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Intranet" },
                    { new Guid("6f61873b-7862-dd7c-ee16-0da2e9700172"), "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Shared, cross-cutting knowledge common to everyone.", true, "commons", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Commons" },
                    { new Guid("7eee4e19-a93e-6eeb-71d6-79876fe9b55c"), "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Configuration and administration guidance.", true, "settings", "SYSTEM", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, null, null, "Settings" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: new Guid("07db6a3f-bde1-d772-d162-36a69635611e"));

            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: new Guid("31d732f1-e968-d4af-d5f1-96eb0df3bf98"));

            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: new Guid("459b165f-0a68-e67f-37cb-0b8720e28494"));

            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: new Guid("46d654b0-5a0b-45d4-6e20-3cc93ea72d2d"));

            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: new Guid("6f61873b-7862-dd7c-ee16-0da2e9700172"));

            migrationBuilder.DeleteData(
                schema: "wikis_wikis",
                table: "Wikis",
                keyColumn: "Id",
                keyValue: new Guid("7eee4e19-a93e-6eeb-71d6-79876fe9b55c"));
        }
    }
}
