namespace WebBanBalo.Model
{
    public class Users
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
         public string UserName { set; get; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }


    }
}
