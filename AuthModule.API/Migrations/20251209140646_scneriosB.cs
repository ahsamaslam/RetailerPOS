using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.API.Migrations
{
    /// <inheritdoc />
    public partial class scneriosB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuyerRegistrationType",
                table: "ScenarioMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerRegistrationType",
                table: "ScenarioMaster");
        }
    }
}
