using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DataMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataMigrations",
                schema: "plutus",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMigrations", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "plutus",
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[] { "d9b8c7b0-3d64-4a5e-9b3f-3e8c9e8b1f5a", "Fixed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataMigrations",
                schema: "plutus");

            migrationBuilder.DeleteData(
                schema: "plutus",
                table: "Categories",
                keyColumn: "Id",
                keyValue: "d9b8c7b0-3d64-4a5e-9b3f-3e8c9e8b1f5a");
        }
    }
}
