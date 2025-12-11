using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWatchlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSeries_AppWhatchlist_WatchlistId",
                table: "AppSeries");

            migrationBuilder.DropTable(
                name: "AppWhatchlist");

            migrationBuilder.DropIndex(
                name: "IX_AppSeries_WatchlistId",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "WatchlistId",
                table: "AppSeries");

            migrationBuilder.CreateTable(
                name: "AppWatchlistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerieId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWatchlistItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppWatchlistItems");

            migrationBuilder.AddColumn<int>(
                name: "WatchlistId",
                table: "AppSeries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppWhatchlist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWhatchlist", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSeries_WatchlistId",
                table: "AppSeries",
                column: "WatchlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSeries_AppWhatchlist_WatchlistId",
                table: "AppSeries",
                column: "WatchlistId",
                principalTable: "AppWhatchlist",
                principalColumn: "Id");
        }
    }
}
