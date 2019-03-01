using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.WatchLater
{
    public partial class Update20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_watchLaters_ID",
                table: "watchLaters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_watchLaters",
                table: "watchLaters");

            migrationBuilder.DropIndex(
                name: "IX_watchLaters_UserID",
                table: "watchLaters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_watchLaters",
                table: "watchLaters",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_watchLaters",
                table: "watchLaters");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_watchLaters_ID",
                table: "watchLaters",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_watchLaters",
                table: "watchLaters",
                columns: new[] { "UserID", "FileID" });

            migrationBuilder.CreateIndex(
                name: "IX_watchLaters_UserID",
                table: "watchLaters",
                column: "UserID");
        }
    }
}
