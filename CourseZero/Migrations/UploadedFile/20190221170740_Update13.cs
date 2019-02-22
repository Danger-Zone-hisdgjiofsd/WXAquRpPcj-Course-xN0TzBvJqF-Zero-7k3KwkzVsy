using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.UploadedFile
{
    public partial class Update13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    File_Name = table.Column<string>(maxLength: 256, nullable: true),
                    File_Typename = table.Column<string>(nullable: true),
                    File_Description = table.Column<string>(maxLength: 10240, nullable: true),
                    Related_courseID = table.Column<int>(nullable: false),
                    Course_Prefix = table.Column<string>(nullable: true),
                    Uploader_UserID = table.Column<int>(nullable: false),
                    Upload_Time = table.Column<DateTime>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    DisLikes = table.Column<int>(nullable: false),
                    Words_for_Search = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadedFiles");
        }
    }
}
