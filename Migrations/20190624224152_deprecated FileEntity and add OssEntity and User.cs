using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OSApiInterface.Migrations
{
    public partial class deprecatedFileEntityandaddOssEntityandUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_entities");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Email = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_id", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "OssEntities",
                columns: table => new
                {
                    OssEntityId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ObjectId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    IsDirectory = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oss_id", x => x.OssEntityId);
                    table.ForeignKey(
                        name: "FK_OssEntities_file_metas_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "file_metas",
                        principalColumn: "Global",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OssEntities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OssEntities_ObjectId",
                table: "OssEntities",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "idx_oss_path",
                table: "OssEntities",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_OssEntities_UserId",
                table: "OssEntities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_user_email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OssEntities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "file_entities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DirectoryId = table.Column<int>(nullable: true),
                    Global = table.Column<long>(nullable: false),
                    IsDirectory = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Share = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true)
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
    }
}
