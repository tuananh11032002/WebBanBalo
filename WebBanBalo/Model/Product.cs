using System.Drawing;
using System.Text.Json.Serialization;

namespace WebBanBalo.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Soluong { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CategoryId { get; set; }
        public Category Categories { get; set; }

        [JsonIgnore]
        public ICollection<ProductImage> Images { get; set; }

        public ICollection<ColorProduct> Colors { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }



    }
}
