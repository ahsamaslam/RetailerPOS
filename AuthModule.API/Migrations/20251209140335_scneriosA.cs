using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.API.Migrations
{
    /// <inheritdoc />
    public partial class scneriosA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScenarioMaster",
                table: "ScenarioMaster");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "ScenarioMaster");

            migrationBuilder.AlterColumn<string>(
                name: "ScenarioId",
                table: "ScenarioMaster",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScenarioMaster",
                table: "ScenarioMaster",
                column: "ScenarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScenarioMaster",
                table: "ScenarioMaster");

            migrationBuilder.AlterColumn<string>(
                name: "ScenarioId",
                table: "ScenarioMaster",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "ScenarioMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScenarioMaster",
                table: "ScenarioMaster",
                column: "ID");
        }
    }
}
