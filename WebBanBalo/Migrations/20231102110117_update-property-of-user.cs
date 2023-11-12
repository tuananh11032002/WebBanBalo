using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanBalo.Migrations
{
    /// <inheritdoc />
    public partial class updatepropertyofuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 11, 2, 18, 1, 16, 973, DateTimeKind.Local).AddTicks(6334),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 11, 2, 17, 54, 13, 704, DateTimeKind.Local).AddTicks(4360));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 11, 2, 17, 54, 13, 704, DateTimeKind.Local).AddTicks(4360),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 11, 2, 18, 1, 16, 973, DateTimeKind.Local).AddTicks(6334));
        }
    }
}
