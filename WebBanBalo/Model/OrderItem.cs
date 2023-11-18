namespace WebBanBalo.Model
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float Discount { get; set; }
        public float Price { get; set; }
        public bool IsReview { get; set; } = false;
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
