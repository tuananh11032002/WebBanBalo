using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppSetting _appSetting;
        private readonly DataContext _dataContext;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper autoMapper,IOptionsMonitor<AppSetting> monitor,DataContext dataContext)
        {
            _dataContext = dataContext;
            _userRepository = userRepository;
            _mapper = autoMapper;
            _appSetting= monitor.CurrentValue;
        }
        [HttpPost("Login")]
        public IActionResult Validate([FromBody] LoginModel loginModel)
        {
            var user = _userRepository.getUser(loginModel);
            if (user == null)
            {
                return Ok(
                    new ResponseApiToken
                    {
                        Message = "User not found",

                        Success = false
                    }
                    );
            }
            return Ok(
                new ResponseApiToken {User= loginModel, Success = true, Message = "Token Created", token = _userRepository.GenerateToken(user) }

                );
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_userRepository.getUsers());
        }
        [HttpGet("{userid}")]
        public IActionResult GetUser( int userid)
        {
            return Ok(_userRepository.getUser(userid));
        }


        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] UsersDto users)
        {
            if (users == null) return BadRequest("Hãy nhập thông tin đầy đủ");
            if (_userRepository.getUser(users.UserName)) return BadRequest("User da ton tai");
            var user = _mapper.Map<Users>(users);
            return Ok(_userRepository.addUser(user));
        }
        [HttpPost("RenewToken")]
        public IActionResult RenewToken(TokenModel model)
        {
            var secretKey = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
            var jwtSecurityHandler = new JwtSecurityTokenHandler();
            var tokenParams = new TokenValidationParameters { ValidateAudience = false, 
                ValidateIssuer = false, 
                ValidateIssuerSigningKey=true, 
                IssuerSigningKey=new SymmetricSecurityKey(secretKey),ClockSkew=TimeSpan.Zero,
                ValidateLifetime=false //khong kt token het han, van tiep tuc chay maf khong out ra
                                       };

            
            try {
                //check 1: format token 
                var tokenverifyction = jwtSecurityHandler.ValidateToken(model.AccessToken, tokenParams, out var validated);

                //check2: thuat toan ma hoa 
                if (validated is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return Ok(new ResponseApiToken { Message="invalid token",Success=false});
                    }
                }
                //check3: access token expire
                var utcExpireDate = long.Parse(tokenverifyction.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate= ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ResponseApiToken
                    {
                        Success = false,
                        Message = "Access token has not yet expired"
                    });
                }

                //check 4: Check refreshtoken exist in DB
                var storedToken = _dataContext.RefreshTokens.FirstOrDefault(x => x.Token == model.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ResponseApiToken
                    {
                        Success = false,
                        Message = "Refresh token does not exist"
                    });
                }

                //check 5: check refreshToken is used/revoked?
                if (storedToken.IsUsed)
                {
                    return Ok(new ResponseApiToken
                    {
                        Success = false,
                        Message = "Refresh token has been used"
                    });
                }
                if (storedToken.IsRevoked)
                {
                    return Ok(new ResponseApiToken
                    {
                        Success = false,
                        Message = "Refresh token has been revoked"
                    });
                }

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenverifyction.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return Ok(new ResponseApiToken
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    });
                }

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _dataContext.Update(storedToken);
                 _dataContext.SaveChanges();

                //create new token
                var user =  _dataContext.Users.SingleOrDefault(nd => nd.Id == storedToken.UserId);
                var token =  _userRepository.GenerateToken(user);
                return Ok(new ResponseApiToken { Message = "success", token = token, Success = true });

            }
            catch
            {
                return BadRequest(new ResponseApiToken { Message = "something went wrong", token = null, Success = false });
            }
            
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval=dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
