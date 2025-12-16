using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class cp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provience_Provienceid",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "Provienceid",
                table: "Cities",
                newName: "ProvienceId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_Provienceid",
                table: "Cities",
                newName: "IX_Cities_ProvienceId");

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provience_ProvienceId",
                table: "Cities",
                column: "ProvienceId",
                principalTable: "Provience",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provience_ProvienceId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "ProvienceId",
                table: "Cities",
                newName: "Provienceid");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_ProvienceId",
                table: "Cities",
                newName: "IX_Cities_Provienceid");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provience_Provienceid",
                table: "Cities",
                column: "Provienceid",
                principalTable: "Provience",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
