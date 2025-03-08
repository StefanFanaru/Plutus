using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequisitionsIsConfirmedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                schema: "plutus",
                table: "Requisitions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                schema: "plutus",
                table: "Requisitions");
        }
    }
}
