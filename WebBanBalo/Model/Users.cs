using WebBanBalo.ModelOther;

namespace WebBanBalo.Model
{
    public enum UserStatus
    {
        Active, 
        Pending,
        Inactive
    }
    public enum UserRole
    {
        Admin,
        Customer
    }
    public class Users
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
         public string UserName { set; get; }
        public string Image { set; get; }
        public string? Phone { set; get; }= string.Empty;
        public UserGender Gender { set; get; }

        public string Password { get; set; }
        public UserRole Role { get; set; } 
        public UserStatus Status { get; set; }
        public DateTime? BirthDay { get; set; }

        public DateTime CreatedDate { get; set; }
        public ICollection<Notification> Notification { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
        public ICollection<Review> Reviews { get; set; }

        public Users()
        {
            Status = UserStatus.Active;
            CreatedDate = DateTime.Now;
            Gender = UserGender.Male;
            Email = String.Empty;
            Role = UserRole.Customer;
            string[] imagePaths = new string[]
                  {
                        "images\\account-female.png",
                        "images\\account-male.png",
                        "images\\account-male-v2.png"
                  };

            // Chọn ngẫu nhiên một đường dẫn từ danh sách
            Random random = new Random();
            int randomIndex = random.Next(0, imagePaths.Length);
            Image = imagePaths[randomIndex];


        }

    }
}
