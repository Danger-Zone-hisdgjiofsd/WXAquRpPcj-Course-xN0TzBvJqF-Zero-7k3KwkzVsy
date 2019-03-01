using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseZero.Migrations.WatchLater
{
    public partial class Update18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "watchLaters",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    FileID = table.Column<int>(nullable: false),
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchLaters", x => new { x.UserID, x.FileID });
                    table.UniqueConstraint("AK_watchLaters_ID", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_watchLaters_UserID",
                table: "watchLaters",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "watchLaters");
        }
    }
}
