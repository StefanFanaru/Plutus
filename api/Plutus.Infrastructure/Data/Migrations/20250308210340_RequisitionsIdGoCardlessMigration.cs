using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequisitionsIdGoCardlessMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoCardlessRequisitionId",
                schema: "plutus",
                table: "Requisitions",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoCardlessRequisitionId",
                schema: "plutus",
                table: "Requisitions");
        }
    }
}
