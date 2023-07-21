namespace WebBanBalo.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ImageReplace { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }

    }
}
