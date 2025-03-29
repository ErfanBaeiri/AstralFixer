using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugFixer.DataLayer.Migrations
{
    public partial class InitialCatalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmailSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "From", "Password" },
                values: new object[] { "astralanswer@gmail.com", "iapy ykva kedv torx" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmailSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "From", "Password" },
                values: new object[] { "bugfixer.toplearn@gmail.com", "strong@password" });
        }
    }
}
