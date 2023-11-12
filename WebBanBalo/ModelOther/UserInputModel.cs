namespace WebBanBalo.ModelOther
{
    public class UserInputModel
    {

        public int Id { get; set; } 
        public string HoTen { set; get; }
        public string Phone { set; get; }   
        public string Email { set; get; }   
        public string Gender { set; get; }
         public IFormFile Image { set; get; }   
    }
}
