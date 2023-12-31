﻿using System.Drawing;
using System.Text.Json.Serialization;

namespace WebBanBalo.Model
{

    public enum StatusProduct
    {
        Publish, Scheduled, Inactive
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public float Discount { get; set; }
        
        public StatusProduct Status { get; set; }
        public int Soluong { get; set; }

        public bool Stock { get; set; }
        public int TotalProduct { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CategoryId { get; set; }
 

        public Category Categories { get; set; }

        public ICollection<ProductImage> Images { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
        

        public Product()
        {
            Price = 0;
            Discount=0;
            Status = StatusProduct.Publish;
        }
    }
}
