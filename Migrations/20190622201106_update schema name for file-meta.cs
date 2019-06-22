using Microsoft.EntityFrameworkCore.Migrations;

namespace OSApiInterface.Migrations
{
    public partial class updateschemanameforfilemeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileMetas",
                table: "FileMetas");

            migrationBuilder.RenameTable(
                name: "FileMetas",
                newName: "file_metas");

            migrationBuilder.AddPrimaryKey(
                name: "pk_fm_global",
                table: "file_metas",
                column: "Global");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_fm_global",
                table: "file_metas");

            migrationBuilder.RenameTable(
                name: "file_metas",
                newName: "FileMetas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileMetas",
                table: "FileMetas",
                column: "Global");
        }
    }
}
