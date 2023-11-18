using WebBanBalo.Model;

namespace WebBanBalo.ModelOther
{
    public class UserCreateInputAdmin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? hoTen { get; set; }
        public string Email { get; set; }
        public UserRole UserRole { get; set; }

    }
}
