using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanBalo.Migrations
{
    /// <inheritdoc />
    public partial class addpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SenderUserId",
                table: "Message",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    vnp_Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_BankCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_BankTranNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_CardType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_OrderInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_PayDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_TmnCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_TransactionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_TransactionStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vnp_TxnRef = table.Column<int>(type: "int", nullable: true),
                    vnp_SecureHash = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderId",
                table: "Payment",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Order");

            migrationBuilder.AlterColumn<int>(
                name: "SenderUserId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
