using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.UploadHist
{
    public partial class Update14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "File_Description",
                table: "UploadHistories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Procesed_ErrorMsg",
                table: "UploadHistories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File_Description",
                table: "UploadHistories");

            migrationBuilder.DropColumn(
                name: "Procesed_ErrorMsg",
                table: "UploadHistories");
        }
    }
}
