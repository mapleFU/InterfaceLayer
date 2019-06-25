using Microsoft.EntityFrameworkCore.Migrations;

namespace OSApiInterface.Migrations
{
    public partial class moveaclauthoritytoOssEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acl",
                table: "file_metas");

            migrationBuilder.AddColumn<int>(
                name: "Access",
                table: "OssEntities",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Access",
                table: "OssEntities");

            migrationBuilder.AddColumn<string>(
                name: "Acl",
                table: "file_metas",
                nullable: true);
        }
    }
}
