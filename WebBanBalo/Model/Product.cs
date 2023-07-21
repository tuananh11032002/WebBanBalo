namespace WebBanBalo.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }

    }
}
