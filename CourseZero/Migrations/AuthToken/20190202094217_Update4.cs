using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.AuthToken
{
    public partial class Update4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthToken",
                table: "AuthToken");

            migrationBuilder.RenameTable(
                name: "AuthToken",
                newName: "AuthTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthTokens",
                table: "AuthTokens",
                column: "Token");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthTokens",
                table: "AuthTokens");

            migrationBuilder.RenameTable(
                name: "AuthTokens",
                newName: "AuthToken");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthToken",
                table: "AuthToken",
                column: "Token");
        }
    }
}
