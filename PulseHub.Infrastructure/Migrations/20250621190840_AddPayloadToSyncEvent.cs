using Microsoft.EntityFrameworkCore.Migrations;

namespace Pulsehub.Infrastructure.Migrations
{
    public partial class AddPayloadToSyncEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Payload",
                table: "SyncEvents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payload",
                table: "SyncEvents");
        }
    }
}
