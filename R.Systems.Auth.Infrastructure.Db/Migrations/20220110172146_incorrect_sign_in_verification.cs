using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace R.Systems.Auth.Infrastructure.Db.Migrations
{
    public partial class incorrect_sign_in_verification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "refresh_token_expire_date_time_utc",
                schema: "user",
                table: "user",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_incorrect_sign_in_date_time_utc",
                schema: "user",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "num_of_incorrect_sign_in",
                schema: "user",
                table: "user",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_incorrect_sign_in_date_time_utc",
                schema: "user",
                table: "user");

            migrationBuilder.DropColumn(
                name: "num_of_incorrect_sign_in",
                schema: "user",
                table: "user");

            migrationBuilder.AlterColumn<DateTime>(
                name: "refresh_token_expire_date_time_utc",
                schema: "user",
                table: "user",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
