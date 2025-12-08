using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class fbrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CustomerCode",
                table: "SalesMasters",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hsCode",
                table: "SalesMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "extraTax",
                table: "SalesDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "fedPayable",
                table: "SalesDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "furtherTax",
                table: "SalesDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "hsCode",
                table: "SalesDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "otherTax",
                table: "SalesDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "saleType",
                table: "SalesDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sroItemSerialNo",
                table: "SalesDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sroScheduleNo",
                table: "SalesDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "uoM",
                table: "SalesDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Register",
                table: "Customers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hsCode",
                table: "SalesMasters");

            migrationBuilder.DropColumn(
                name: "extraTax",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "fedPayable",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "furtherTax",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "hsCode",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "otherTax",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "saleType",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "sroItemSerialNo",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "sroScheduleNo",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "uoM",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "Register",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerCode",
                table: "SalesMasters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
