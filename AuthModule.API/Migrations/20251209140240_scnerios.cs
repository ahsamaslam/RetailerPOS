using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.API.Migrations
{
    /// <inheritdoc />
    public partial class scnerios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScenarioMaster",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenarioId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScenarioName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SroScheduleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SroItemSerialNo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioMaster", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioMaster");
        }
    }
}
