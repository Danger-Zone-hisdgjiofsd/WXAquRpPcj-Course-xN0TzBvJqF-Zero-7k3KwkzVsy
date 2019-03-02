using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.ProfileComments
{
    public partial class Update23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "ProfileComments");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_receiver_UserID",
                table: "ProfileComments",
                newName: "IX_ProfileComments_receiver_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfileComments",
                table: "ProfileComments",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfileComments",
                table: "ProfileComments");

            migrationBuilder.RenameTable(
                name: "ProfileComments",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_ProfileComments_receiver_UserID",
                table: "Comments",
                newName: "IX_Comments_receiver_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "ID");
        }
    }
}
