using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.UploadHist
{
    public partial class Update9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "File_typename",
                table: "UploadHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File_typename",
                table: "UploadHistories");
        }
    }
}
