using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class cust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "Vendors",
                newName: "STRN");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Customers",
                newName: "STRN");

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Vendors",
                type: "int",
                nullable: true);

           

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Register",
                table: "Vendors",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "openDate",
                table: "Vendors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "openingBalance",
                table: "Vendors",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "companyID",
                table: "SalesMasters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "saleCode",
                table: "SalesMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);
 

            migrationBuilder.AddColumn<int>(
                name: "Cityid",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "openDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "openingBalance",
                table: "Customers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Provience",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provience", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provienceid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.id);
                    table.ForeignKey(
                        name: "FK_Cities_Provience_Provienceid",
                        column: x => x.Provienceid,
                        principalTable: "Provience",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Cityid",
                table: "Vendors",
                column: "Cityid");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cityid",
                table: "Customers",
                column: "Cityid");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Provienceid",
                table: "Cities",
                column: "Provienceid");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Cities_Cityid",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Cities_Cityid",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Provience");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_Cityid",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Cityid",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Cityid",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Register",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "openDate",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "openingBalance",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "companyID",
                table: "SalesMasters");

            migrationBuilder.DropColumn(
                name: "saleCode",
                table: "SalesMasters");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Cityid",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "openDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "openingBalance",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "STRN",
                table: "Vendors",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "STRN",
                table: "Customers",
                newName: "City");
        }
    }
}
