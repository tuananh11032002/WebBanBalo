namespace WebBanBalo.Dto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ImageReplace { get; set; }
        public float TotalEarning { get; set; }
        public int TotalProduct { get; set; }
        public string Title { get; set; } = string.Empty;


    }
}
