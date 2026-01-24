using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvTracker.Migrations
{
    /// <inheritdoc />
    public partial class Added_Notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedSeries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "AppNotifications");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOmdbCheck",
                table: "AppSeries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AppNotifications",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppNotifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "AppNotifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RelatedEntityId",
                table: "AppNotifications",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AppNotifications",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AppNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppNotifications",
                table: "AppNotifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppNotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNotificationPreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppNotificationPreferences_UserId_Type_Channel",
                table: "AppNotificationPreferences",
                columns: new[] { "UserId", "Type", "Channel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppNotificationPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppNotifications",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "LastOmdbCheck",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "RelatedEntityId",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppNotifications");

            migrationBuilder.RenameTable(
                name: "AppNotifications",
                newName: "Notifications");

            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                table: "AppSeries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppSeries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TrackedSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedSeries", x => x.Id);
                });
        }
    }
}
