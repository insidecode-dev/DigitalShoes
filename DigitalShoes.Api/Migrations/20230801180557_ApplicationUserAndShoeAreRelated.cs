using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserAndShoeAreRelated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Shoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shoes_ApplicationUserId",
                table: "Shoes",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes");

            migrationBuilder.DropIndex(
                name: "IX_Shoes_ApplicationUserId",
                table: "Shoes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Shoes");
        }
    }
}
