using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.API.Migrations
{
    /// <inheritdoc />
    public partial class companyscnerio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyScenario",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioMasterId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyScenario", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CompanyScenario_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyScenario_ScenarioMaster_ScenarioMasterId",
                        column: x => x.ScenarioMasterId,
                        principalTable: "ScenarioMaster",
                        principalColumn: "ScenarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyScenario_CompanyId",
                table: "CompanyScenario",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyScenario_ScenarioMasterId",
                table: "CompanyScenario",
                column: "ScenarioMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyScenario");
        }
    }
}
