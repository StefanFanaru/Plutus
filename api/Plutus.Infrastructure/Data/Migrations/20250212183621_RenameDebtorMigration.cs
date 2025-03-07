using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameDebtorMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Obligors_DebitorId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "DebitorId",
                schema: "plutus",
                table: "Transactions",
                newName: "DebtorId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_DebitorId",
                schema: "plutus",
                table: "Transactions",
                newName: "IX_Transactions_DebtorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Obligors_DebtorId",
                schema: "plutus",
                table: "Transactions",
                column: "DebtorId",
                principalSchema: "plutus",
                principalTable: "Obligors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Obligors_DebtorId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "DebtorId",
                schema: "plutus",
                table: "Transactions",
                newName: "DebitorId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_DebtorId",
                schema: "plutus",
                table: "Transactions",
                newName: "IX_Transactions_DebitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Obligors_DebitorId",
                schema: "plutus",
                table: "Transactions",
                column: "DebitorId",
                principalSchema: "plutus",
                principalTable: "Obligors",
                principalColumn: "Id");
        }
    }
}
