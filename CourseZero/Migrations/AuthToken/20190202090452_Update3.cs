using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.AuthToken
{
    public partial class Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthToken",
                columns: table => new
                {
                    Token = table.Column<string>(nullable: false),
                    userID = table.Column<int>(nullable: false),
                    Last_access_IP = table.Column<string>(nullable: true),
                    Last_access_Location = table.Column<string>(nullable: true),
                    Last_access_Time = table.Column<DateTime>(nullable: false),
                    Last_access_Device = table.Column<string>(nullable: true),
                    Last_access_Browser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthToken", x => x.Token);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthToken");
        }
    }
}
