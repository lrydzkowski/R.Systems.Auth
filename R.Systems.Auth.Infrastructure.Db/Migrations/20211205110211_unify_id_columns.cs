using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace R.Systems.Auth.Infrastructure.Db.Migrations
{
    public partial class unify_id_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "user",
                table: "user",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "role_id",
                schema: "user",
                table: "role",
                newName: "id");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                schema: "user",
                table: "user",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:IdentitySequenceOptions", "'2', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                schema: "user",
                table: "user",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "user",
                table: "role",
                newName: "role_id");

            migrationBuilder.AlterColumn<long>(
                name: "user_id",
                schema: "user",
                table: "user",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'2', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
