using WebBanBalo.Model;

namespace WebBanBalo.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public float? Discount { get; set; } = 0;
        public StatusProduct Status { get; set; } = StatusProduct.Publish;
        public int TotalProduct { get; set; } = 0;

        public List<string> Image { get; set; }
        public int Soluong { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Stock { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }
        public ICollection<Review> Reviews { get; set; }


    }
}
