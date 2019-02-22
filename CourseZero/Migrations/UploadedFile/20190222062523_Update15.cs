using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.UploadedFile
{
    public partial class Update15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Binary",
                table: "UploadedFiles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Stored_Internally",
                table: "UploadedFiles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Binary",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "Stored_Internally",
                table: "UploadedFiles");
        }
    }
}
