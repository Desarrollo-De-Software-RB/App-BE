using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvTracker.Migrations
{
    /// <inheritdoc />
    public partial class Added_ProfilePicture_To_IdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "AbpUsers",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AbpUsers");
        }
    }
}
