using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedAppearanceClassandProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppearanceId",
                table: "UsersTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Appearance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EyeColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HairColor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appearance", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersTable_AppearanceId",
                table: "UsersTable",
                column: "AppearanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTable_Appearance_AppearanceId",
                table: "UsersTable",
                column: "AppearanceId",
                principalTable: "Appearance",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTable_Appearance_AppearanceId",
                table: "UsersTable");

            migrationBuilder.DropTable(
                name: "Appearance");

            migrationBuilder.DropIndex(
                name: "IX_UsersTable_AppearanceId",
                table: "UsersTable");

            migrationBuilder.DropColumn(
                name: "AppearanceId",
                table: "UsersTable");
        }
    }
}
