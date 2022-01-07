using Microsoft.EntityFrameworkCore.Migrations;

namespace R.Systems.Auth.Infrastructure.Db.Migrations
{
    public partial class add_user_email_unique_constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                schema: "user",
                table: "user",
                column: "email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_email",
                schema: "user",
                table: "user");
        }
    }
}
