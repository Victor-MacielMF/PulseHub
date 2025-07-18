﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Pulsehub.Infrastructure.Migrations
{
    public partial class AddQueueMessageStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "QueueMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "QueueMessages");
        }
    }
}
