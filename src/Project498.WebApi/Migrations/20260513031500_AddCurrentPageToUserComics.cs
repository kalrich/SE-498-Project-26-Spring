using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project498.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentPageToUserComics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentPage",
                table: "UserComics",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPage",
                table: "UserComics");
        }
    }
}
