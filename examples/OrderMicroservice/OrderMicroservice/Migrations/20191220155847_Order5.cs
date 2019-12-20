using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class Order5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_order_items");

            migrationBuilder.DropIndex(
                name: "IX_tbl_order_items_OrderEntityOrderId",
                table: "tbl_order_items");

            migrationBuilder.DropColumn(
                name: "OrderEntityOrderId",
                table: "tbl_order_items");

            migrationBuilder.AlterColumn<string>(
                name: "order_id",
                table: "tbl_order_items",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_orders_order_id",
                table: "tbl_order_items",
                column: "order_id",
                principalTable: "tbl_orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_orders_order_id",
                table: "tbl_order_items");

            migrationBuilder.AlterColumn<string>(
                name: "order_id",
                table: "tbl_order_items",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "OrderEntityOrderId",
                table: "tbl_order_items",
                type: "varchar(64)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_OrderEntityOrderId",
                table: "tbl_order_items",
                column: "OrderEntityOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_order_items",
                column: "OrderEntityOrderId",
                principalTable: "tbl_orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
