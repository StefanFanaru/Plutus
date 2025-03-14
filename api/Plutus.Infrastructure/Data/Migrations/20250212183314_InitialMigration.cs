using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "plutus");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "plutus",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Obligors",
                schema: "plutus",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "plutus",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", maxLength: 36, precision: 18, scale: 4, nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreditorId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    DebitorId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "plutus",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Obligors_CreditorId",
                        column: x => x.CreditorId,
                        principalSchema: "plutus",
                        principalTable: "Obligors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Obligors_DebitorId",
                        column: x => x.DebitorId,
                        principalSchema: "plutus",
                        principalTable: "Obligors",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "plutus",
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "67afadc8-75d1-a130-a6cb-29cab5fd0d03", "Food" },
                    { "67afadc8-75d1-a130-a6cb-29cab5fd0d04", "Vice" },
                    { "67afadc8-75d1-a130-a6cb-29cab5fd0d05", "Transport" },
                    { "67afadc8-75d1-a130-a6cb-29cab5fd0d07", "Fun" },
                    { "67afadc8-75d1-a130-a6cb-29cab5fd0d08", "House" },
                    { "67afadc8-75d1-a130-a6cb-29cab5fd0d09", "Gift" },
                    { "67afadc8-75d1-a130-a6cb-29cab5fd0d33", "Life" },
                    { "f47ac10b-58cc-4372-a567-0e02b2c3d479", "Uncategorized" },
                    { "d9b8c7b0-3d64-4a5e-9b3f-3e8c9e8b1f5a", "Fixed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "plutus",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Obligors_Name",
                schema: "plutus",
                table: "Obligors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                schema: "plutus",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreditorId",
                schema: "plutus",
                table: "Transactions",
                column: "CreditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DebitorId",
                schema: "plutus",
                table: "Transactions",
                column: "DebitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "plutus");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "plutus");

            migrationBuilder.DropTable(
                name: "Obligors",
                schema: "plutus");
        }
    }
}
