using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class Order2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_items");

            migrationBuilder.DropIndex(
                name: "IX_tbl_items_OrderEntityOrderId",
                table: "tbl_items");

            migrationBuilder.DropColumn(
                name: "OrderEntityOrderId",
                table: "tbl_items");

            migrationBuilder.CreateTable(
                name: "tbl_order_items",
                columns: table => new
                {
                    order_item_id = table.Column<string>(nullable: false),
                    order_id = table.Column<string>(nullable: true),
                    item_id = table.Column<string>(nullable: true),
                    OrderEntityOrderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_order_items", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK_tbl_order_items_tbl_items_item_id",
                        column: x => x.item_id,
                        principalTable: "tbl_items",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_order_items_tbl_orders_OrderEntityOrderId",
                        column: x => x.OrderEntityOrderId,
                        principalTable: "tbl_orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_order_items_tbl_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "tbl_orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_item_id",
                table: "tbl_order_items",
                column: "item_id",
                unique: true,
                filter: "[item_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_OrderEntityOrderId",
                table: "tbl_order_items",
                column: "OrderEntityOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items",
                column: "order_id",
                unique: true,
                filter: "[order_id] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_order_items");

            migrationBuilder.AddColumn<string>(
                name: "OrderEntityOrderId",
                table: "tbl_items",
                type: "varchar(64)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_items_OrderEntityOrderId",
                table: "tbl_items",
                column: "OrderEntityOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_items",
                column: "OrderEntityOrderId",
                principalTable: "tbl_orders",
                principalColumn: "order_id");
        }
    }
}
