using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanBalo.Migrations
{
    /// <inheritdoc />
    public partial class imgaereplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageReplace",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageReplace",
                table: "Category");
        }
    }
}
