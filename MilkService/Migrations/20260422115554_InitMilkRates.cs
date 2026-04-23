using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilkService.Migrations
{
    /// <inheritdoc />
    public partial class InitMilkRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CustomerRate",
                table: "Milks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SupplierRate",
                table: "Milks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Milks_CustomerId",
                table: "Milks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Milks_SupplierId",
                table: "Milks",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Milks_Customers_CustomerId",
                table: "Milks",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Milks_Suppliers_SupplierId",
                table: "Milks",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Milks_Customers_CustomerId",
                table: "Milks");

            migrationBuilder.DropForeignKey(
                name: "FK_Milks_Suppliers_SupplierId",
                table: "Milks");

            migrationBuilder.DropIndex(
                name: "IX_Milks_CustomerId",
                table: "Milks");

            migrationBuilder.DropIndex(
                name: "IX_Milks_SupplierId",
                table: "Milks");

            migrationBuilder.DropColumn(
                name: "CustomerRate",
                table: "Milks");

            migrationBuilder.DropColumn(
                name: "SupplierRate",
                table: "Milks");
        }
    }
}
