using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace instagram.Migrations
{
    /// <inheritdoc />
    public partial class StergeColoanaImagine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagine",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagine",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
