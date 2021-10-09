using Microsoft.EntityFrameworkCore.Migrations;

namespace GraniteHouseV2.Migrations
{
    public partial class addApplicationTypeToProductModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationTypeId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Product_ApplicationTypeId",
                table: "Product",
                column: "ApplicationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product",
                column: "ApplicationTypeId",
                principalTable: "ApplicationType",
                principalColumn: "ApplicationTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ApplicationTypeId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeId",
                table: "Product");
        }
    }
}
