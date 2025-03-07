using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TransactionOriginalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalTransactionId",
                schema: "plutus",
                table: "Transactions",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OriginalTransactionId",
                schema: "plutus",
                table: "Transactions",
                column: "OriginalTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Transactions_OriginalTransactionId",
                schema: "plutus",
                table: "Transactions",
                column: "OriginalTransactionId",
                principalSchema: "plutus",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Transactions_OriginalTransactionId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_OriginalTransactionId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OriginalTransactionId",
                schema: "plutus",
                table: "Transactions");
        }
    }
}
