using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations
{
    public partial class Update5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "email_verification_issue_datetime",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "password_change_hash",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_change_new_password",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "password_change_request_datatime",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email_verification_issue_datetime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "password_change_hash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "password_change_new_password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "password_change_request_datatime",
                table: "Users");
        }
    }
}
