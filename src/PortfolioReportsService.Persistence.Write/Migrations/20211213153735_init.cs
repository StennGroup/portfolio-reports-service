using Microsoft.EntityFrameworkCore.Migrations;

namespace PortfolioReportsService.Persistence.Write.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON", true);
            migrationBuilder.Sql("ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
