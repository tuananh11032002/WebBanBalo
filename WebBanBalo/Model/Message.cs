namespace WebBanBalo.Model
{
    public class Message
    {
        public int Id { get; set; }
        public int? SenderUserId { get; set; }
        public int? ReceiverUserId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Users? ReceiveUser { get; set; }
        public Users? SenderUser { get; set; }

    }
}
