using WebBanBalo.Dto;

namespace WebBanBalo.Model
{
    public class OrderItemDto
    {
        public float Price { get; set; }
        public int Quantity { get; set; }
        public ProductDto Product { get; set; }
    }
}
