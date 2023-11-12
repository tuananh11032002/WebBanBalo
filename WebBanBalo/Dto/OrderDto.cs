namespace WebBanBalo.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public float TotalAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
    }
}
