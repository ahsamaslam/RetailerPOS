using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class eftax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "extraTaxP",
                table: "SalesDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "furtherTaxP",
                table: "SalesDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "extraTaxP",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "furtherTaxP",
                table: "SalesDetails");
        }
    }
}
