using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class yearadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_OpeningBalance_Year_Product",
                table: "OpeningBalances");

            migrationBuilder.DropColumn(
                name: "Product",
                table: "OpeningBalances");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "SalesMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "PurchaseMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductID",
                table: "OpeningBalances",
                type: "int",
                maxLength: 200,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "UX_OpeningBalance_Year_Product",
                table: "OpeningBalances",
                columns: new[] { "Year", "ProductID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_OpeningBalance_Year_Product",
                table: "OpeningBalances");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "SalesMasters");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "PurchaseMasters");

            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "OpeningBalances");

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "OpeningBalances",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UX_OpeningBalance_Year_Product",
                table: "OpeningBalances",
                columns: new[] { "Year", "Product" },
                unique: true);
        }
    }
}
