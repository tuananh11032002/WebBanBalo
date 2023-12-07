namespace WebBanBalo.Dto
{
    public class RecentChatUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public string LastMessageContent { get; set; }
        public string LastMessageSentTimeString { get; set; }
    }

}
