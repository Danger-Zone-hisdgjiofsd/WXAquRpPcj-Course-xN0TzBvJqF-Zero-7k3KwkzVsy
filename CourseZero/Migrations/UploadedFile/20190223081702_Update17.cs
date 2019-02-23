using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.UploadedFile
{
    public partial class Update17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_Likes",
                table: "UploadedFiles",
                column: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_Upload_Time",
                table: "UploadedFiles",
                column: "Upload_Time");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UploadedFiles_Likes",
                table: "UploadedFiles");

            migrationBuilder.DropIndex(
                name: "IX_UploadedFiles_Upload_Time",
                table: "UploadedFiles");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_Upload_Time_Likes_Related_courseID_File_Typename",
                table: "UploadedFiles",
                columns: new[] { "Upload_Time", "Likes", "Related_courseID", "File_Typename" });
        }
    }
}
