using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentMicroservice.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_payments",
                columns: table => new
                {
                    payment_id = table.Column<string>(type: "varchar(64)", nullable: false),
                    order_id = table.Column<string>(type: "varchar(64)", nullable: false),
                    amount_payable = table.Column<double>(nullable: false),
                    payment_status = table.Column<string>(type: "varchar(20)", nullable: false),
                    username = table.Column<string>(type: "varchar(50)", nullable: false),
                    created_ts = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_payments", x => x.payment_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_payments_order_id",
                table: "tbl_payments",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_payments_payment_id",
                table: "tbl_payments",
                column: "payment_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_payments");
        }
    }
}
