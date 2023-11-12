namespace WebBanBalo.Model
{
    public enum UserStatus
    {
        Active, 
        Pending,
        Inactive
    }
    public class Users
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
         public string UserName { set; get; }
        public string Image { set; get; }
        public string? Phone { set; get; }= string.Empty;
        public string? Gender { set; get; }=string.Empty;

        public string Password { get; set; }
        public string Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? BirthDay { get; set; }

        public DateTime CreatedDate { get; set; }
        public ICollection<Notification> Notification { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }

        public Users()
        {
            Status = UserStatus.Active;
            CreatedDate = DateTime.Now;
        }

    }
}
