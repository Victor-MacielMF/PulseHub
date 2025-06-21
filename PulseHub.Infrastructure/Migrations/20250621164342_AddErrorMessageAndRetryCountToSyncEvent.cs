using Microsoft.EntityFrameworkCore.Migrations;

namespace Pulsehub.Infrastructure.Migrations
{
    public partial class AddErrorMessageAndRetryCountToSyncEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "SyncEvents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "SyncEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "SyncEvents");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "SyncEvents");
        }
    }
}
