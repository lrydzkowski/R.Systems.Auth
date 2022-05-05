using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace R.Systems.Auth.Infrastructure.Db.Migrations
{
    public partial class add_refresh_token_cols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                schema: "user",
                table: "user",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expire_date_time_utc",
                schema: "user",
                table: "user",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refresh_token",
                schema: "user",
                table: "user");

            migrationBuilder.DropColumn(
                name: "refresh_token_expire_date_time_utc",
                schema: "user",
                table: "user");
        }
    }
}
