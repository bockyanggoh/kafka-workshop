using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderMicroservice.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    item_id = table.Column<string>(type: "varchar(64)", nullable: false),
                    item_name = table.Column<string>(type: "varchar(100)", nullable: true),
                    item_type = table.Column<string>(type: "varchar(50)", nullable: false),
                    created_ts = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.item_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_item_id",
                table: "Items",
                column: "item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
