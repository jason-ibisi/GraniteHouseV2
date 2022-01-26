using Microsoft.EntityFrameworkCore.Migrations;

namespace GraniteHouseV2_DataAccess.Migrations
{
    public partial class UpdatePrimaryKeyColumnOfOrderDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderDetailsId",
                table: "OrderDetail",
                newName: "OrderDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "OrderDetail",
                newName: "OrderDetailsId");
        }
    }
}
