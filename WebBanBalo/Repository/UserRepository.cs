using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebBanBalo.Data;
using WebBanBalo.Interface;
using WebBanBalo.Migrations;
using WebBanBalo.Model;

namespace WebBanBalo.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly AppSetting _appSetting;
        public UserRepository(DataContext dataContext, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _dataContext = dataContext;
             _appSetting= optionsMonitor.CurrentValue;
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
                    //new Claim(ClaimTypes.Role, "admin")

                }),
                Expires=DateTime.UtcNow.AddMinutes(1),
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
                ExpiredAt = DateTime.UtcNow.AddMinutes(1)
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
            var user = _dataContext.Users.FirstOrDefault(p => p.UserName == userName);
            return user != null;
        }

        public bool addUser(Users user)
        {
            _dataContext.Users.Add(user);
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

        public Users getUser(int userid)
        {
            return _dataContext.Users.Where(p => p.Id == userid).FirstOrDefault();
        }
    }
}
