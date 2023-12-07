using System.Text.Json.Serialization;

namespace WebBanBalo.Model
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime DatePosted { get; set; }

        public int ProductId { get; set; }
        [JsonIgnore]

        public Product Product { get; set; }
        public int? UserId { get; set; }
        [JsonIgnore]

        public Users? User { get; set; }

     
    }


}
