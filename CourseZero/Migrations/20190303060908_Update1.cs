using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthTokens",
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
                    table.PrimaryKey("PK_AuthTokens", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Prefix = table.Column<string>(nullable: true),
                    Subject_Name = table.Column<string>(nullable: true),
                    Course_Code = table.Column<string>(nullable: true),
                    Course_Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProfileComments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    sender_UserID = table.Column<int>(nullable: false),
                    receiver_UserID = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 2048, nullable: true),
                    posted_dateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileComments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    CourseID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.UserID, x.CourseID });
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    File_Name = table.Column<string>(maxLength: 256, nullable: true),
                    File_Typename = table.Column<string>(maxLength: 10, nullable: true),
                    File_Description = table.Column<string>(maxLength: 10240, nullable: true),
                    Related_courseID = table.Column<int>(nullable: false),
                    Course_Prefix = table.Column<string>(nullable: true),
                    Uploader_UserID = table.Column<int>(nullable: false),
                    Upload_Time = table.Column<DateTime>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    DisLikes = table.Column<int>(nullable: false),
                    Words_for_Search = table.Column<string>(nullable: true),
                    Stored_Internally = table.Column<bool>(nullable: false),
                    Binary = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UploadHistories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    File_Name = table.Column<string>(maxLength: 256, nullable: true),
                    File_typename = table.Column<string>(nullable: true),
                    File_Description = table.Column<string>(maxLength: 10240, nullable: true),
                    Related_courseID = table.Column<int>(nullable: false),
                    Uploader_UserID = table.Column<int>(nullable: false),
                    Upload_Time = table.Column<DateTime>(nullable: false),
                    Processed = table.Column<bool>(nullable: false),
                    Processed_Success = table.Column<bool>(nullable: false),
                    Procesed_ErrorMsg = table.Column<int>(nullable: false),
                    Processed_FileID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadHistories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    password_hash = table.Column<string>(nullable: true),
                    password_salt = table.Column<string>(nullable: true),
                    email_verified = table.Column<bool>(nullable: false),
                    email_verifying_hash = table.Column<string>(nullable: true),
                    email_verification_issue_datetime = table.Column<DateTime>(nullable: false),
                    password_change_new_password = table.Column<string>(nullable: true),
                    password_change_hash = table.Column<string>(nullable: true),
                    password_change_request_datatime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "watchLaters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    FileID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchLaters", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileComments_receiver_UserID",
                table: "ProfileComments",
                column: "receiver_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserID",
                table: "Subscriptions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_Likes",
                table: "UploadedFiles",
                column: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_Upload_Time",
                table: "UploadedFiles",
                column: "Upload_Time");

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_username",
                table: "Users",
                column: "username",
                unique: true,
                filter: "[username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthTokens");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "ProfileComments");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "UploadHistories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "watchLaters");
        }
    }
}
