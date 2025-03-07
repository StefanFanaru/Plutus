using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserFkMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                schema: "plutus",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Obligors_UserId",
                schema: "plutus",
                table: "Obligors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GoCardlessRequests_UserId",
                schema: "plutus",
                table: "GoCardlessRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceAudits_UserId",
                schema: "plutus",
                table: "BalanceAudits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceAudits_Users_UserId",
                schema: "plutus",
                table: "BalanceAudits",
                column: "UserId",
                principalSchema: "plutus",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GoCardlessRequests_Users_UserId",
                schema: "plutus",
                table: "GoCardlessRequests",
                column: "UserId",
                principalSchema: "plutus",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Obligors_Users_UserId",
                schema: "plutus",
                table: "Obligors",
                column: "UserId",
                principalSchema: "plutus",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserId",
                schema: "plutus",
                table: "Transactions",
                column: "UserId",
                principalSchema: "plutus",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceAudits_Users_UserId",
                schema: "plutus",
                table: "BalanceAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_GoCardlessRequests_Users_UserId",
                schema: "plutus",
                table: "GoCardlessRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Obligors_Users_UserId",
                schema: "plutus",
                table: "Obligors");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                schema: "plutus",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Obligors_UserId",
                schema: "plutus",
                table: "Obligors");

            migrationBuilder.DropIndex(
                name: "IX_GoCardlessRequests_UserId",
                schema: "plutus",
                table: "GoCardlessRequests");

            migrationBuilder.DropIndex(
                name: "IX_BalanceAudits_UserId",
                schema: "plutus",
                table: "BalanceAudits");
        }
    }
}
