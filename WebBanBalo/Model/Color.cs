namespace WebBanBalo.Model
{
    public class Color
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public string Name { get; set; }
        public  ICollection<ColorProduct> Products { get; set; }

    }
}
