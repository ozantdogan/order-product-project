using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OTD.Repository.Migrations
{
    /// <inheritdoc />
    public partial class User_confirmvalues_delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationExpireDate",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationCode",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmationExpireDate",
                table: "Users",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
