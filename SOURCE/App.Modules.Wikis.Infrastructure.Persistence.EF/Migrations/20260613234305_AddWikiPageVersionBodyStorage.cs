using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWikiPageVersionBodyStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "WikiPageVersions",
                schema: "wikis_wikis",
                comment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; is the sink-agnostic body locator (ADR-018N §2.6) addressing the version's raw body bytes in whichever body-storage sink is configured — a WikiPageVersionBody satellite row (Database sink), an immutable object-store blob (ObjectStore sink), or an external content-repo file (FileSystem sink). is the sink-independent content hash of those bytes and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\" regardless of where the bytes physically live.",
                oldComment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; points at an immutable object-store blob (reusing Sys.Infrastructure.Media). is the content hash of that blob and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\".");

            migrationBuilder.AlterColumn<string>(
                name: "ContentHash",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "The content hash of the body. Computed identically across every body sink so a mirrored copy can be verified equal; used for staleness/drift detection and as the subject a verifiable endorsement (ADR-018M) is pinned to.",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldComment: "The content hash of the body blob. Used for staleness/drift detection and as the subject a verifiable endorsement (ADR-018M) is pinned to.");

            migrationBuilder.AlterColumn<Guid>(
                name: "BodyBlobId",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The sink-agnostic body locator for this version's raw body bytes (ADR-018N §2.6). Its concrete meaning depends on the active body sink: the PK of the WikiPageVersionBody satellite row (Database sink), the object-store blob id (ObjectStore sink), or a deterministic handle from which the content-repo file path is derived (FileSystem sink). Replacing content means a new locator on a new version row, never a mutation of an existing one.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Identifier of the immutable object-store blob holding this version's raw body bytes. Replacing content means a new blob id, never a mutation of the existing blob.");

            migrationBuilder.CreateTable(
                name: "WikiPageVersionBodies",
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
                    WikiPageVersionFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to the owning (1:1). Under the Database sink this also equals the version's body locator (BodyBlobId), so the body can be fetched directly from the version's stored locator without a second lookup key. Navigable, so the suffix is FK, not Id."),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "The raw text body of the version, in the format declared by the version's ContentFormatKey (ADR-018E). Stored as unbounded Unicode text so it is full-text indexable in place.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiPageVersionBodies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikiPageVersionBodies_WikiPageVersions_WikiPageVersionFK",
                        column: x => x.WikiPageVersionFK,
                        principalSchema: "wikis_wikis",
                        principalTable: "WikiPageVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The bytes-bearing satellite row for the Database body storage sink (ADR-018N §2.2, Seam 1). It holds the raw text body of exactly one , 1:1, via a foreign key into the version — it never alters the core row, in keeping with the ADR-018 §2.7 additive-tables seam.");

            migrationBuilder.CreateIndex(
                name: "IX_WikiPageVersionBodies_WikiPageVersionFK",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                column: "WikiPageVersionFK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPageVersionBody_Id",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiPageVersionBody_RecordState",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                column: "RecordState");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WikiPageVersionBodies",
                schema: "wikis_wikis");

            migrationBuilder.AlterTable(
                name: "WikiPageVersions",
                schema: "wikis_wikis",
                comment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; points at an immutable object-store blob (reusing Sys.Infrastructure.Media). is the content hash of that blob and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\".",
                oldComment: "An immutable snapshot of a 's content at a point in time. This is the heart of the ADR-018 immutability invariant: a version is never edited in place. \"Editing\" appends a new WikiPageVersion and repoints . The body itself is not stored inline; is the sink-agnostic body locator (ADR-018N §2.6) addressing the version's raw body bytes in whichever body-storage sink is configured — a WikiPageVersionBody satellite row (Database sink), an immutable object-store blob (ObjectStore sink), or an external content-repo file (FileSystem sink). is the sink-independent content hash of those bytes and is what an Open-Badges / VC endorsement (ADR-018M) pins to, so a badge can be proven to be \"for this exact version\" regardless of where the bytes physically live.");

            migrationBuilder.AlterColumn<string>(
                name: "ContentHash",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "The content hash of the body blob. Used for staleness/drift detection and as the subject a verifiable endorsement (ADR-018M) is pinned to.",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldComment: "The content hash of the body. Computed identically across every body sink so a mirrored copy can be verified equal; used for staleness/drift detection and as the subject a verifiable endorsement (ADR-018M) is pinned to.");

            migrationBuilder.AlterColumn<Guid>(
                name: "BodyBlobId",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Identifier of the immutable object-store blob holding this version's raw body bytes. Replacing content means a new blob id, never a mutation of the existing blob.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The sink-agnostic body locator for this version's raw body bytes (ADR-018N §2.6). Its concrete meaning depends on the active body sink: the PK of the WikiPageVersionBody satellite row (Database sink), the object-store blob id (ObjectStore sink), or a deterministic handle from which the content-repo file path is derived (FileSystem sink). Replacing content means a new locator on a new version row, never a mutation of an existing one.");
        }
    }
}
