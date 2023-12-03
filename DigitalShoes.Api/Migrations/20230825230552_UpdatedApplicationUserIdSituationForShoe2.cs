using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedApplicationUserIdSituationForShoe2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shoes_AspNetUsers_ApplicationUserId",
                table: "Shoes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
