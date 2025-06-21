using Microsoft.EntityFrameworkCore.Migrations;

namespace Pulsehub.Infrastructure.Migrations
{
    public partial class AddChannelToQueueMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "QueueMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "QueueMessages");
        }
    }
}
