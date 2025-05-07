using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugFixer.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddStateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Birthdate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GetNewsletter",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_States_ParentId",
                        column: x => x.ParentId,
                        principalTable: "States",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "EmailSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "From", "Password" },
                values: new object[] { "****", "****" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CityId",
                table: "Users",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CountryId",
                table: "Users",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_States_ParentId",
                table: "States",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_States_CityId",
                table: "Users",
                column: "CityId",
                principalTable: "States",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_States_CountryId",
                table: "Users",
                column: "CountryId",
                principalTable: "States",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_States_CityId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_States_CountryId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropIndex(
                name: "IX_Users_CityId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CountryId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Birthdate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GetNewsletter",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "EmailSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "From", "Password" },
                values: new object[] { "****", "****" });
        }
    }
}
