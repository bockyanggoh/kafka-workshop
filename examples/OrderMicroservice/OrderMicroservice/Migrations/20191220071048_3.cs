using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tbl_items_item_name",
                table: "tbl_items",
                column: "item_name",
                unique: true,
                filter: "[item_name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_items_item_name",
                table: "tbl_items");
        }
    }
}
