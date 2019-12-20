using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class Order1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderEntityOrderId",
                table: "tbl_items",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "tbl_orders",
                columns: table => new
                {
                    order_id = table.Column<string>(type: "varchar(64)", nullable: false),
                    username = table.Column<string>(type: "varchar(64)", nullable: true),
                    created_ts = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_orders", x => x.order_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_items_OrderEntityOrderId",
                table: "tbl_items",
                column: "OrderEntityOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_orders_username",
                table: "tbl_orders",
                column: "username");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_items",
                column: "OrderEntityOrderId",
                principalTable: "tbl_orders",
                principalColumn: "order_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_items_tbl_orders_OrderEntityOrderId",
                table: "tbl_items");

            migrationBuilder.DropTable(
                name: "tbl_orders");

            migrationBuilder.DropIndex(
                name: "IX_tbl_items_OrderEntityOrderId",
                table: "tbl_items");

            migrationBuilder.DropColumn(
                name: "OrderEntityOrderId",
                table: "tbl_items");
        }
    }
}
