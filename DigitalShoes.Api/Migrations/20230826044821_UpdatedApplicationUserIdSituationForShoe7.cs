using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedApplicationUserIdSituationForShoe7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
