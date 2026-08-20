using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Modules.Wikis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignGeneratedIndexNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_WikiTemplateSection_PrecedenceOrder",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "UX_WikiTemplateSections_PrecedenceOrder");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateSection_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "IX_WikiTemplateSections_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateSection_Id",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "IX_WikiTemplateSections_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplate_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "IX_WikiTemplates_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplate_Id",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "IX_WikiTemplates_Id");

            migrationBuilder.RenameIndex(
                name: "UX_WikiTemplateBinding_PrecedenceOrder",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "UX_WikiTemplateBindings_PrecedenceOrder");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateBinding_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "IX_WikiTemplateBindings_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateBinding_Id",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "IX_WikiTemplateBindings_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Wiki_RecordState",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "IX_Wikis_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_Wiki_Id",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "IX_Wikis_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersion_RecordState",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "IX_WikiPageVersions_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersion_Id",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "IX_WikiPageVersions_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersionBody_RecordState",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "IX_WikiPageVersionBodies_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersionBody_Id",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "IX_WikiPageVersionBodies_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPage_RecordState",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "IX_WikiPages_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPage_Id",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "IX_WikiPages_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiNodeStyle_RecordState",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "IX_WikiNodeStyles_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiNodeStyle_Id",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "IX_WikiNodeStyles_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiAcl_RecordState",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcls_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiAcl_Id",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcls_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_WikiTemplateSections_PrecedenceOrder",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "UX_WikiTemplateSection_PrecedenceOrder");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateSections_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "IX_WikiTemplateSection_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateSections_Id",
                schema: "wikis_wikis",
                table: "WikiTemplateSections",
                newName: "IX_WikiTemplateSection_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplates_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "IX_WikiTemplate_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplates_Id",
                schema: "wikis_wikis",
                table: "WikiTemplates",
                newName: "IX_WikiTemplate_Id");

            migrationBuilder.RenameIndex(
                name: "UX_WikiTemplateBindings_PrecedenceOrder",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "UX_WikiTemplateBinding_PrecedenceOrder");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateBindings_RecordState",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "IX_WikiTemplateBinding_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiTemplateBindings_Id",
                schema: "wikis_wikis",
                table: "WikiTemplateBindings",
                newName: "IX_WikiTemplateBinding_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Wikis_RecordState",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "IX_Wiki_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_Wikis_Id",
                schema: "wikis_wikis",
                table: "Wikis",
                newName: "IX_Wiki_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersions_RecordState",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "IX_WikiPageVersion_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersions_Id",
                schema: "wikis_wikis",
                table: "WikiPageVersions",
                newName: "IX_WikiPageVersion_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersionBodies_RecordState",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "IX_WikiPageVersionBody_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPageVersionBodies_Id",
                schema: "wikis_wikis",
                table: "WikiPageVersionBodies",
                newName: "IX_WikiPageVersionBody_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPages_RecordState",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "IX_WikiPage_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiPages_Id",
                schema: "wikis_wikis",
                table: "WikiPages",
                newName: "IX_WikiPage_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiNodeStyles_RecordState",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "IX_WikiNodeStyle_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiNodeStyles_Id",
                schema: "wikis_wikis",
                table: "WikiNodeStyles",
                newName: "IX_WikiNodeStyle_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WikiAcls_RecordState",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcl_RecordState");

            migrationBuilder.RenameIndex(
                name: "IX_WikiAcls_Id",
                schema: "wikis_wikis",
                table: "WikiAcls",
                newName: "IX_WikiAcl_Id");
        }
    }
}
