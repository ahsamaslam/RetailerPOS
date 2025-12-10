using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.API.Migrations
{
    /// <inheritdoc />
    public partial class companyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyType",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "edVal",
                table: "Companies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "fedVal",
                table: "Companies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "gstVal",
                table: "Companies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isEd",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isFed",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isGst",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "edVal",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "fedVal",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "gstVal",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "isEd",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "isFed",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "isGst",
                table: "Companies");
        }
    }
}
