namespace WebBanBalo.Model
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }

        public Users User { get; set; }


    }
}
