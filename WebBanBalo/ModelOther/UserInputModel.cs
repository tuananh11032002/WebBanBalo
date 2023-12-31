﻿using WebBanBalo.Model;

namespace WebBanBalo.ModelOther
{
    public class UserInputModel
    {

        public int Id { get; set; } 
        public string HoTen { set; get; }
        public string Phone { set; get; }   
        public string Email { set; get; }   
        public UserGender? Gender { set; get; }
        public IFormFile? Image { set; get; }   

        public UserStatus? UserStatus { set; get; }
    }
}
