using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class saleref : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "SalesMasters");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "SalesMasters");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "SalesDetails");

            migrationBuilder.RenameColumn(
                name: "ItemCode",
                table: "SalesDetails",
                newName: "ItemId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "SalesMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SalesMasters_CustomerID",
                table: "SalesMasters",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDetails_ItemId",
                table: "SalesDetails",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDetails_Items_ItemId",
                table: "SalesDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesMasters_Customers_CustomerID",
                table: "SalesMasters",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesDetails_Items_ItemId",
                table: "SalesDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesMasters_Customers_CustomerID",
                table: "SalesMasters");

            migrationBuilder.DropIndex(
                name: "IX_SalesMasters_CustomerID",
                table: "SalesMasters");

            migrationBuilder.DropIndex(
                name: "IX_SalesDetails_ItemId",
                table: "SalesDetails");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "SalesMasters");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "SalesDetails",
                newName: "ItemCode");

            migrationBuilder.AddColumn<int>(
                name: "CustomerCode",
                table: "SalesMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "SalesMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "SalesDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
