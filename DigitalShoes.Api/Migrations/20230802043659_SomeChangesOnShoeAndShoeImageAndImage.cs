using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShoes.Api.Migrations
{
    /// <inheritdoc />
    public partial class SomeChangesOnShoeAndShoeImageAndImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoeImages");

            migrationBuilder.AddColumn<int>(
                name: "ShoeId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Images_ShoeId",
                table: "Images",
                column: "ShoeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Shoes_ShoeId",
                table: "Images",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Shoes_ShoeId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ShoeId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ShoeId",
                table: "Images");

            migrationBuilder.CreateTable(
                name: "ShoeImages",
                columns: table => new
                {
                    ShoeId = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataStatus = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoeImages", x => new { x.ShoeId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_ShoeImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoeImages_Shoes_ShoeId",
                        column: x => x.ShoeId,
                        principalTable: "Shoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoeImages_ImageId",
                table: "ShoeImages",
                column: "ImageId");
        }
    }
}
