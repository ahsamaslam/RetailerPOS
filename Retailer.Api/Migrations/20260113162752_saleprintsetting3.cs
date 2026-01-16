using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class saleprintsetting3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscountAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountRound",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Footer1Align",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Footer1Margin",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Footer1Padding",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Footer2Align",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Footer2Margin",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Footer2Padding",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Footer3Align",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Footer3Margin",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Footer3Padding",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GSTAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GSTPercentAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GSTPercentRound",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GSTRound",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Header1Align",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Header1Margin",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Header1Padding",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Header2Align",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Header2Margin",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Header2Padding",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Header3Align",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Header3Margin",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Header3Padding",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QtyAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RateAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RateRound",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SrAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalAlign",
                table: "saleInvoiceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRound",
                table: "saleInvoiceSettings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "DiscountRound",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer1Align",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer1Margin",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer1Padding",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer2Align",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer2Margin",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer2Padding",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer3Align",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer3Margin",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Footer3Padding",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "GSTAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "GSTPercentAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "GSTPercentRound",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "GSTRound",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header1Align",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header1Margin",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header1Padding",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header2Align",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header2Margin",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header2Padding",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header3Align",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header3Margin",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "Header3Padding",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "NameAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "QtyAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "RateAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "RateRound",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "SrAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "TotalAlign",
                table: "saleInvoiceSettings");

            migrationBuilder.DropColumn(
                name: "TotalRound",
                table: "saleInvoiceSettings");
        }
    }
}
