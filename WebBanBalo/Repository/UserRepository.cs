using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Migrations;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly AppSetting _appSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserRepository(DataContext dataContext, IOptionsMonitor<AppSetting> optionsMonitor,IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _appSetting= optionsMonitor.CurrentValue;
            _webHostEnvironment = webHostEnvironment;
        }

        public TokenModel GenerateToken(Users user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeybytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
            var description = new SecurityTokenDescriptor { 
                Subject=new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name,user.HoTen),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim("UserName",user.UserName),
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)

                }),
                Expires=DateTime.UtcNow.AddDays(1),
                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(secretKeybytes),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken((description));

            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken { 
                Id= Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(1)
            };

            _dataContext.Add(refreshTokenEntity);
            _dataContext.SaveChanges();
            return new TokenModel { AccessToken = accessToken, RefreshToken = refreshToken };

        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        public Users getUser(LoginModel loginModel)
        {
            return _dataContext.Users.Where(p => p.UserName == loginModel.userName && p.Password == loginModel.password).FirstOrDefault();
        }

        public bool getUser(string userName)
        {
            var user = _dataContext.Users.Where(p => p.UserName == userName).FirstOrDefault();
            return user != null;
        }

        public bool addUser(Users user)
        {
            if(user.HoTen==null) {
                user.HoTen = user.UserName;
            }
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges(true);
            //Lấy user tại đây: 

            Users addedUser = _dataContext.Users.FirstOrDefault(u => u.UserName == user.UserName);


            _dataContext.Order.Add(new Order
            {
                UserId = addedUser.Id,
                
                
            });
            return Save(); 
        }

        private bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved>0 ? true : false;
        }

        public List<Users> getUsers()
        {
            return _dataContext.Users.ToList();
        }

        public async Task<Users> getUser(int userid)
        {
            return await _dataContext.Users.Where(p => p.Id == userid).FirstOrDefaultAsync();
        }

        public bool IsHasFirstMessage(int id)
        {
            try
            {
                var user = _dataContext.Users.Where((p) => p.Id == id).FirstOrDefault();

                if (user!=null)
                {
                    string userRole = user.Role;
                      if(userRole =="admin")
                      {
                            ICollection<Users> users = _dataContext.Users.Where(p=>p.Id!=user.Id).ToList();
                            foreach( var temp in users)
                            {
                                int count =_dataContext.Message.Where(m => (m.SenderUserId == user.Id && m.ReceiverUserId == temp.Id) ||
                                                                (m.SenderUserId == temp.Id && m.ReceiverUserId == user.Id)).Count();
                                if(count == 0)
                                {
                                    _dataContext.Message.Add(new Message()
                                    {
                                        ReceiverUserId = temp.Id,
                                        SenderUserId = user.Id,
                                        Content = "Đây là tin nhắn tự động, nếu bạn cần sự giúp đỡ hãy chat với chúng tôi để được hỗ trợ",
                                        Timestamp = DateTime.Now,
                                    
                                    });
                                    _dataContext.SaveChanges();
                                }

                            }
                      }
                      else
                      {
                            ICollection<Users> users = _dataContext.Users.Where(p => p.Role =="admin").ToList();
                            foreach (var temp in users)
                            {
                                int count = _dataContext.Message.Where(m => (m.SenderUserId == user.Id && m.ReceiverUserId == temp.Id) ||
                                                                (m.SenderUserId == temp.Id && m.ReceiverUserId == user.Id)).Count();
                                if (count == 0)
                                {
                                    _dataContext.Message.Add(new Message()
                                    {
                                        ReceiverUserId = user.Id,
                                        SenderUserId = temp.Id,
                                        Content = "Đây là tin nhắn tự động, nếu bạn cần sự giúp đỡ hãy chat với chúng tôi để được hỗ trợ",
                                        Timestamp = DateTime.Now,

                                    });
                                    _dataContext.SaveChanges();
                                }

                            }
                       }
                      return true;


                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUser(Users user)
        {
            _dataContext.Remove(user);
            return Save();
        }

        public async Task<bool> Update(UserInputModel user)
        {
            try
            {
                var image = user.Image;
                if (image.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string imagePath = Path.Combine("images", uniqueFileName);
                    var imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var userTemp = await _dataContext.Users.Where(p => p.Id == user.Id).FirstAsync();
                    userTemp.HoTen = user.HoTen;
                    userTemp.Email = user.Email;
                    userTemp.Phone = user.Phone;
                    userTemp.Gender = user.Gender;
                    userTemp.Image = imagePath;
                }
                else
                {
                    var userTemp = await _dataContext.Users.FirstAsync(p => p.Id == user.Id);
                    userTemp.HoTen = user.HoTen;
                    userTemp.Email = user.Email;
                    userTemp.Phone = user.Phone;
                    userTemp.Gender = user.Gender;
                }

                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc trả về thông báo lỗi cụ thể
                // Log.Error(ex, "Lỗi xử lý cập nhật người dùng");
                return false;
            }
        }
    }
}
