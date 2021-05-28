using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Api.Migrations
{
    public partial class AddTryNumberToEmailVerifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RecoveryCode",
                table: "EmailVerifications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "TryNumber",
                table: "EmailVerifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TryNumber",
                table: "EmailVerifications");

            migrationBuilder.AlterColumn<int>(
                name: "RecoveryCode",
                table: "EmailVerifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
