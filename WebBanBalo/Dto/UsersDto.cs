namespace WebBanBalo.Dto
{
    public class UsersDto
    {
        public int Id { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string UserName { set; get; }
        public string? Image { set; get; } = string.Empty;
        public string? Phone { set; get; } = string.Empty;
        public string? Gender { set; get; } = string.Empty;
        public string Password { get; set; }

        public string? Role { get; set; } = "user";
        public DateTime? BirthDay { get; set; }

        public DateTime CreatedDate { get; set; }= DateTime.Now;
    }
}
