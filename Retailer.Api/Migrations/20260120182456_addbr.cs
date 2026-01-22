using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class addbr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "OpeningBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_BranchId",
                table: "OpeningBalances",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpeningBalances_Branches_BranchId",
                table: "OpeningBalances",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpeningBalances_Branches_BranchId",
                table: "OpeningBalances");

            migrationBuilder.DropIndex(
                name: "IX_OpeningBalances_BranchId",
                table: "OpeningBalances");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "OpeningBalances");
        }
    }
}
