﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class Order8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_items_item_id",
                table: "tbl_order_items");

            migrationBuilder.DropIndex(
                name: "IX_tbl_order_items_item_id",
                table: "tbl_order_items");

            migrationBuilder.AlterColumn<string>(
                name: "item_id",
                table: "tbl_order_items",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_item_id",
                table: "tbl_order_items",
                column: "item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_order_items_item_id",
                table: "tbl_order_items");

            migrationBuilder.AlterColumn<string>(
                name: "item_id",
                table: "tbl_order_items",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_item_id",
                table: "tbl_order_items",
                column: "item_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_items_item_id",
                table: "tbl_order_items",
                column: "item_id",
                principalTable: "tbl_items",
                principalColumn: "item_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
