using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAPI.Migrations
{
    /// <inheritdoc />
    public partial class addednewPropsforUserBackground : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "UserBackground",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductionType",
                table: "UserBackground",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VideoUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserBackgroundId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoUrls_UserBackground_UserBackgroundId",
                        column: x => x.UserBackgroundId,
                        principalTable: "UserBackground",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoUrls_UserBackgroundId",
                table: "VideoUrls",
                column: "UserBackgroundId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoUrls");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "UserBackground");

            migrationBuilder.DropColumn(
                name: "ProductionType",
                table: "UserBackground");
        }
    }
}
