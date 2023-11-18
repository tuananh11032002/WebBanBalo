using WebBanBalo.Dto;

namespace WebBanBalo.Model
{
    public class ResponseApiToken
    {

        public string UserId { get; set; }
        public bool Success { get; set; }
        public object token { get; set; }
        public string  Message { get; set; }
        public  string userName { get; set; }
        public UserRole Role { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }


}
}
