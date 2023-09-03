using WebBanBalo.Dto;

namespace WebBanBalo.Model
{
    public class ResponseApiToken
    {
       
        public bool Success { get; set; }
        public object token { get; set; }
        public string  Message { get; set; }
        public LoginModel User { get; set; }
                
}
}
