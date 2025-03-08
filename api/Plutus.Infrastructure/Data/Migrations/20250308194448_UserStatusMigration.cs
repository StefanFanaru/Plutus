﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plutus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserStatusMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "plutus",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "plutus",
                table: "Users");
        }
    }
}
