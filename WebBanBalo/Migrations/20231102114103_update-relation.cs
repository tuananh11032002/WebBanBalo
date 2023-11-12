using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanBalo.Migrations
{
    /// <inheritdoc />
    public partial class updaterelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_ReceiverUserId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_SenderUserId",
                table: "Message");

            migrationBuilder.AlterColumn<float>(
                name: "Discount",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 11, 2, 18, 41, 3, 232, DateTimeKind.Local).AddTicks(8655),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 11, 2, 18, 1, 16, 973, DateTimeKind.Local).AddTicks(6334));

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_ReceiverUserId",
                table: "Message",
                column: "ReceiverUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_SenderUserId",
                table: "Message",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_ReceiverUserId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_SenderUserId",
                table: "Message");

            migrationBuilder.AlterColumn<float>(
                name: "Discount",
                table: "Product",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 11, 2, 18, 1, 16, 973, DateTimeKind.Local).AddTicks(6334),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 11, 2, 18, 41, 3, 232, DateTimeKind.Local).AddTicks(8655));

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_ReceiverUserId",
                table: "Message",
                column: "ReceiverUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_SenderUserId",
                table: "Message",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
