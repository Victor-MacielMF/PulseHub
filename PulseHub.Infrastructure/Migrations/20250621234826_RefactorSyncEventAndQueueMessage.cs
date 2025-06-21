using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pulsehub.Infrastructure.Migrations
{
    public partial class RefactorSyncEventAndQueueMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "SyncEvents");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "SyncEvents");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "QueueMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAttemptAt",
                table: "QueueMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "QueueMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "QueueMessages");

            migrationBuilder.DropColumn(
                name: "LastAttemptAt",
                table: "QueueMessages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "QueueMessages");

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
    }
}
