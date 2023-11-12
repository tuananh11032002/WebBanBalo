namespace WebBanBalo.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ImageReplace { get; set; }
        public string Title { get; set; }= string.Empty;
       

        public ICollection<Product> Products { get; set; }




    }
}
