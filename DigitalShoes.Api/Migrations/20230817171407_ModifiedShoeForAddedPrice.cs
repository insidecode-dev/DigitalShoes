using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedShoeForAddedPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Shoes",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Shoes");
        }
    }
}
