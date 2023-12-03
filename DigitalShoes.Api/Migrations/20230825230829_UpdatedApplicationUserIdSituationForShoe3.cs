using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedApplicationUserIdSituationForShoe3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Shoes_ShoeId",
                table: "Reviews",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id");
        }
    }
}
