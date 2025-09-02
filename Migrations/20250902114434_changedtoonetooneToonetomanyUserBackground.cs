using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedtoonetooneToonetomanyUserBackground : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBackground_UserId",
                table: "UserBackground");

            migrationBuilder.CreateIndex(
                name: "IX_UserBackground_UserId",
                table: "UserBackground",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBackground_UserId",
                table: "UserBackground");

            migrationBuilder.CreateIndex(
                name: "IX_UserBackground_UserId",
                table: "UserBackground",
                column: "UserId",
                unique: true);
        }
    }
}
