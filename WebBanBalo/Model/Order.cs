namespace WebBanBalo.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public float TotalAmount { get; set; }
        public bool Done { get; set; }
        public DateTime CreatedAt { get; set; }
        public Users User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
