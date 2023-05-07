using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pl_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMarkerToUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Markers_Users_UserId",
                table: "Markers");

            migrationBuilder.DropIndex(
                name: "IX_Markers_UserId",
                table: "Markers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Markers");

            migrationBuilder.AddColumn<int>(
                name: "MarkerId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MarkerId",
                table: "Users",
                column: "MarkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Markers_MarkerId",
                table: "Users",
                column: "MarkerId",
                principalTable: "Markers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Markers_MarkerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_MarkerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MarkerId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Markers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Markers_UserId",
                table: "Markers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Markers_Users_UserId",
                table: "Markers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
