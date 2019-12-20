using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "tbl_items");

            migrationBuilder.RenameIndex(
                name: "IX_Items_item_id",
                table: "tbl_items",
                newName: "IX_tbl_items_item_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_items",
                table: "tbl_items",
                column: "item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_items",
                table: "tbl_items");

            migrationBuilder.RenameTable(
                name: "tbl_items",
                newName: "Items");

            migrationBuilder.RenameIndex(
                name: "IX_tbl_items_item_id",
                table: "Items",
                newName: "IX_Items_item_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "item_id");
        }
    }
}
