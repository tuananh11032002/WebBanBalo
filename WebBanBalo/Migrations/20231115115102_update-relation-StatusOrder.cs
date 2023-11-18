using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanBalo.Migrations
{
    /// <inheritdoc />
    public partial class updaterelationStatusOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusUpdate_Order_OrderId",
                table: "OrderStatusUpdate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatusUpdate",
                table: "OrderStatusUpdate");

            migrationBuilder.RenameTable(
                name: "OrderStatusUpdate",
                newName: "OrderStatusUpdates");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusUpdate_OrderId",
                table: "OrderStatusUpdates",
                newName: "IX_OrderStatusUpdates_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatusUpdates",
                table: "OrderStatusUpdates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusUpdates_Order_OrderId",
                table: "OrderStatusUpdates",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusUpdates_Order_OrderId",
                table: "OrderStatusUpdates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatusUpdates",
                table: "OrderStatusUpdates");

            migrationBuilder.RenameTable(
                name: "OrderStatusUpdates",
                newName: "OrderStatusUpdate");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusUpdates_OrderId",
                table: "OrderStatusUpdate",
                newName: "IX_OrderStatusUpdate_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatusUpdate",
                table: "OrderStatusUpdate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusUpdate_Order_OrderId",
                table: "OrderStatusUpdate",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
