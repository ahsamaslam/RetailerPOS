using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class saleprintsetting1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "saleInvoiceSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orientation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageWidthMM = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PageHeightMM = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Header1Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Header1Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Header1FontSize = table.Column<int>(type: "int", nullable: true),
                    Header2Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Header2FontSize = table.Column<int>(type: "int", nullable: true),
                    Header3Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Header3FontSize = table.Column<int>(type: "int", nullable: true),
                    ShowGST = table.Column<bool>(type: "bit", nullable: false),
                    SrLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SrFontSize = table.Column<int>(type: "int", nullable: true),
                    NameLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFontSize = table.Column<int>(type: "int", nullable: true),
                    RateLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RateFontSize = table.Column<int>(type: "int", nullable: true),
                    QtyLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyFontSize = table.Column<int>(type: "int", nullable: true),
                    DiscountLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountFontSize = table.Column<int>(type: "int", nullable: true),
                    TotalLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalFontSize = table.Column<int>(type: "int", nullable: true),
                    GSTLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSTFontSize = table.Column<int>(type: "int", nullable: true),
                    GSTPercentLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSTPercentFontSize = table.Column<int>(type: "int", nullable: true),
                    Footer1Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Footer1FontSize = table.Column<int>(type: "int", nullable: true),
                    Footer2Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Footer2FontSize = table.Column<int>(type: "int", nullable: true),
                    Footer3Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Footer3FontSize = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    companyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    branchID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saleInvoiceSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "saleInvoiceSettings");
        }
    }
}
