using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace instagram.Migrations
{
    /// <inheritdoc />
    public partial class RenameUrlToImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Posts",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Posts",
                newName: "Url");
        }
    }
}
