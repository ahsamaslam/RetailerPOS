using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class addbank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Provience",
                newName: "Name");
 

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Cityid",
                table: "Customers",
                newName: "IX_Customers_CityId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Cities",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Cities",
                newName: "Id");
 
       
          
 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Cities_CityId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Cities_CityId1",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CityId1",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CityId1",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "Vendors",
                newName: "Cityid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Provience",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Provience",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "Customers",
                newName: "Cityid");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_CityId",
                table: "Customers",
                newName: "IX_Customers_Cityid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Cities",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cities",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "Cityid",
                table: "Vendors",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Cityid",
                table: "Vendors",
                column: "Cityid");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Cities_Cityid",
                table: "Customers",
                column: "Cityid",
                principalTable: "Cities",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Cities_Cityid",
                table: "Vendors",
                column: "Cityid",
                principalTable: "Cities",
                principalColumn: "id");
        }
    }
}
