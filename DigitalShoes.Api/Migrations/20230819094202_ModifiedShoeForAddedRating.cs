using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedShoeForAddedRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Shoes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Shoes");
        }
    }
}
