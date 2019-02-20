using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.UploadHist
{
    public partial class Update12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Related_courseID",
                table: "UploadHistories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Related_courseID",
                table: "UploadHistories");
        }
    }
}
