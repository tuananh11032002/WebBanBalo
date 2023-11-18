namespace WebBanBalo.Model
{
    public class OrderStatusUpdate
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
