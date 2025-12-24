using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailer.Api.Migrations
{
    /// <inheritdoc />
    public partial class CustomerPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    bankCode = table.Column<int>(type: "int", nullable: false),
                    bankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    taxPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    taxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    whtPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    whtAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    companyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    userCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPayment_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethod",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayment_PaymentMethodId",
                table: "CustomerPayment",
                column: "PaymentMethodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPayment");

            migrationBuilder.DropTable(
                name: "PaymentMethod");
        }
    }
}
