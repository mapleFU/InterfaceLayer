using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OSApiInterface.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file_entities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Global = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    IsDirectory = table.Column<bool>(nullable: false),
                    DirectoryId = table.Column<int>(nullable: true),
                    Share = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_entities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_entities_file_entities_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "file_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "file_entities",
                columns: new[] { "Id", "DirectoryId", "Global", "IsDirectory", "Name", "Share", "Type" },
                values: new object[] { -1, null, 0L, false, "/", true, "folder" });

            migrationBuilder.CreateIndex(
                name: "idx_fe_directory",
                table: "file_entities",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "idx_fe_name",
                table: "file_entities",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_entities");
        }
    }
}
