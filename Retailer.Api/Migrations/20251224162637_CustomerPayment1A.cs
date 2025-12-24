using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class CustomerPayment1A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "bankCode",
                table: "CustomerPayment",
                newName: "BanksId");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "CustomerPayment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayment_BanksId",
                table: "CustomerPayment",
                column: "BanksId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayment_CustomerId",
                table: "CustomerPayment",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPayment_Banks_BanksId",
                table: "CustomerPayment",
                column: "BanksId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPayment_Customers_CustomerId",
                table: "CustomerPayment",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPayment_Banks_BanksId",
                table: "CustomerPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPayment_Customers_CustomerId",
                table: "CustomerPayment");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPayment_BanksId",
                table: "CustomerPayment");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPayment_CustomerId",
                table: "CustomerPayment");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "CustomerPayment");

            migrationBuilder.RenameColumn(
                name: "BanksId",
                table: "CustomerPayment",
                newName: "bankCode");
        }
    }
}
