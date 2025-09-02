using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedPersonalBackgroundPropsForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonalBackgroundId",
                table: "UsersTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonalBackground",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ethnicity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaturalAccent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalBackground", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersTable_PersonalBackgroundId",
                table: "UsersTable",
                column: "PersonalBackgroundId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTable_PersonalBackground_PersonalBackgroundId",
                table: "UsersTable",
                column: "PersonalBackgroundId",
                principalTable: "PersonalBackground",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTable_PersonalBackground_PersonalBackgroundId",
                table: "UsersTable");

            migrationBuilder.DropTable(
                name: "PersonalBackground");

            migrationBuilder.DropIndex(
                name: "IX_UsersTable_PersonalBackgroundId",
                table: "UsersTable");

            migrationBuilder.DropColumn(
                name: "PersonalBackgroundId",
                table: "UsersTable");
        }
    }
}
