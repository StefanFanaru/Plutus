using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ObligorIdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Obligors_CreditorId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Obligors_DebtorId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CreditorId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreditorId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "DebtorId",
                schema: "plutus",
                table: "Transactions",
                newName: "ObligorId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_DebtorId",
                schema: "plutus",
                table: "Transactions",
                newName: "IX_Transactions_ObligorId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCredit",
                schema: "plutus",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Obligors_ObligorId",
                schema: "plutus",
                table: "Transactions",
                column: "ObligorId",
                principalSchema: "plutus",
                principalTable: "Obligors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Obligors_ObligorId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsCredit",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "ObligorId",
                schema: "plutus",
                table: "Transactions",
                newName: "DebtorId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_ObligorId",
                schema: "plutus",
                table: "Transactions",
                newName: "IX_Transactions_DebtorId");

            migrationBuilder.AddColumn<string>(
                name: "CreditorId",
                schema: "plutus",
                table: "Transactions",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreditorId",
                schema: "plutus",
                table: "Transactions",
                column: "CreditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Obligors_CreditorId",
                schema: "plutus",
                table: "Transactions",
                column: "CreditorId",
                principalSchema: "plutus",
                principalTable: "Obligors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Obligors_DebtorId",
                schema: "plutus",
                table: "Transactions",
                column: "DebtorId",
                principalSchema: "plutus",
                principalTable: "Obligors",
                principalColumn: "Id");
        }
    }
}
