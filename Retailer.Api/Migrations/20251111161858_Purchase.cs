using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class Purchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "PurchaseDetails");

            migrationBuilder.RenameColumn(
                name: "SupplierID",
                table: "PurchaseMasters",
                newName: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_BranchId",
                table: "PurchaseMasters",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_LoginId",
                table: "PurchaseMasters",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_VendorID",
                table: "PurchaseMasters",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetails_ItemId",
                table: "PurchaseDetails",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Items_ItemId",
                table: "PurchaseDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseMasters_Branches_BranchId",
                table: "PurchaseMasters",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseMasters_Logins_LoginId",
                table: "PurchaseMasters",
                column: "LoginId",
                principalTable: "Logins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseMasters_Vendors_VendorID",
                table: "PurchaseMasters",
                column: "VendorID",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Items_ItemId",
                table: "PurchaseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseMasters_Branches_BranchId",
                table: "PurchaseMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseMasters_Logins_LoginId",
                table: "PurchaseMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseMasters_Vendors_VendorID",
                table: "PurchaseMasters");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseMasters_BranchId",
                table: "PurchaseMasters");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseMasters_LoginId",
                table: "PurchaseMasters");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseMasters_VendorID",
                table: "PurchaseMasters");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDetails_ItemId",
                table: "PurchaseDetails");

            migrationBuilder.RenameColumn(
                name: "VendorID",
                table: "PurchaseMasters",
                newName: "SupplierID");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "PurchaseDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
