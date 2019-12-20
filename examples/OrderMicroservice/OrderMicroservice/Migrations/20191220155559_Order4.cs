using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class Order4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_orders_order_id",
                table: "tbl_order_items");

            migrationBuilder.DropIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items");

            migrationBuilder.AlterColumn<string>(
                name: "order_id",
                table: "tbl_order_items",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "OrderEntityOrderId",
                table: "tbl_order_items",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items",
                column: "order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_order_items",
                column: "OrderEntityOrderId",
                principalTable: "tbl_orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_order_items");

            migrationBuilder.DropIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items");

            migrationBuilder.AlterColumn<string>(
                name: "order_id",
                table: "tbl_order_items",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "OrderEntityOrderId",
                table: "tbl_order_items",
                type: "varchar(64)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items",
                column: "order_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_order_items",
                column: "OrderEntityOrderId",
                principalTable: "tbl_orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_orders_order_id",
                table: "tbl_order_items",
                column: "order_id",
                principalTable: "tbl_orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
