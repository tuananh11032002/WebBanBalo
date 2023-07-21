using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanBalo.Migrations
{
    /// <inheritdoc />
    public partial class updatedatacontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "TotalAmount",
                table: "Order",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<bool>(
                name: "Done",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Done",
                table: "Order");

            migrationBuilder.AlterColumn<float>(
                name: "TotalAmount",
                table: "Order",
                type: "real",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);
        }
    }
}
