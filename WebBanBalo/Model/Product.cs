using System.Drawing;
using System.Text.Json.Serialization;

namespace WebBanBalo.Model
{
    public enum PaymentStatus {
        Paid, Pending, Failed, Cancelled
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public float Discount { get; set; }
        public float FeeShip { get; set; } = 0;
        
        public PaymentStatus PaymentStatus { get; set; }
        public string Status { get; set; }  = string.Empty;
        public int Soluong { get; set; }

        public bool Stock { get; set; }
        public int TotalProduct { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CategoryId { get; set; }
        [JsonIgnore]

        public Category Categories { get; set; }

        public ICollection<ProductImage> Images { get; set; }
        [JsonIgnore]

        public ICollection<ColorProduct> Colors { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }



    }
}
