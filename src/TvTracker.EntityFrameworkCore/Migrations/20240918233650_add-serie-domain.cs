using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvTracker.Migrations
{
    /// <inheritdoc />
    public partial class addseriedomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    Year = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Rated = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Released = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Runtime = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Director = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, defaultValue: "N/A"),
                    Writer = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, defaultValue: "N/A"),
                    Actors = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, defaultValue: "N/A"),
                    Plot = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Awards = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Poster = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Metascore = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "N/A"),
                    IMDBRating = table.Column<float>(type: "real", maxLength: 4, nullable: false),
                    IMDBVotes = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IMDBID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TotalSeasons = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSeries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSeries");
        }
    }
}
