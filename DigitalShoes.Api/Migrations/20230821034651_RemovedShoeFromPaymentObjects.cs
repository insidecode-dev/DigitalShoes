using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemovedShoeFromPaymentObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentObjects_Shoes_ShoeId",
                table: "PaymentObjects");

            migrationBuilder.DropIndex(
                name: "IX_PaymentObjects_ShoeId",
                table: "PaymentObjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PaymentObjects_ShoeId",
                table: "PaymentObjects",
                column: "ShoeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentObjects_Shoes_ShoeId",
                table: "PaymentObjects",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
