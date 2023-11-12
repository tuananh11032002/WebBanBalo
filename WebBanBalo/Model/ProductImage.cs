using System.Text.Json.Serialization;

namespace WebBanBalo.Model
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FilePath { get; set; }



        public int ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
