using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugFixer.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddUseCountToTblTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailSettings",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.AddColumn<int>(
                name: "UseCount",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseCount",
                table: "Tags");

            migrationBuilder.InsertData(
                table: "EmailSettings",
                columns: new[] { "Id", "CreateDate", "DisplayName", "EnableSSL", "From", "IsDefault", "IsDelete", "Password", "Port", "SMTP" },
                values: new object[] { 1L, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BugFixer", true, "****", true, false, "****", 587, "smtp.gmail.com" });
        }
    }
}
