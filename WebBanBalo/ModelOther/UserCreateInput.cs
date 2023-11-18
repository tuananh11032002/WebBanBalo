using System.ComponentModel.DataAnnotations;

namespace WebBanBalo.ModelOther
{
    public enum UserGender
    {
        Male, Female
    }
    public class UserCreateInput
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public UserGender Gender { get; set; }
        public UserCreateInput()
        {
            Gender = UserGender.Male;
        }

    }
}
