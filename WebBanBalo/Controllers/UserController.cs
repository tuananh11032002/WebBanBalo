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
using WebBanBalo.ModelOther;

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
        
        
        /// <summary>
        /// API for User Login
        /// </summary>
        /// <remarks>
        /// api login
        /// </remarks>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        
        public async Task<IActionResult> Validate([FromBody] LoginModel loginModel)
        {
            try
            {
                if (loginModel.userName.IsNullOrEmpty() || loginModel.password.IsNullOrEmpty())
                {
                    return BadRequest("Bạn đang để trống 1 trường quan trọng, kiểm tra lại username và password");
                }

                ValueReturn user = await _userRepository.getUser(loginModel);

                if (user.Status == false)
                {
                    return BadRequest(user.Message);
                }
                else
                {
                    Users userTemp = (Users)user.Data;
                bool resultCheck = _userRepository.IsHasFirstMessage(userTemp.Id);
                    return Ok(
                        new ResponseApiToken { UserId = userTemp.Id.ToString(), userName = loginModel.userName, Role = userTemp.Role, 
                            Success = true, Message = "Token Created", token = _userRepository.GenerateToken(userTemp), Image = userTemp.Image, DisplayName = userTemp.HoTen });
                }
            }
            catch (Exception ex) 
            {
                return StatusCode(500,ex.Message);
            }
        }

        /// <summary>
        /// Api get all user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserStatus? userStatus, [FromQuery] UserRole? userRole, [FromQuery] string? search, [FromQuery] int pageIndex=1, [FromQuery] int pageSize=10)
        {
            try
            {
                ValueReturn result = await _userRepository.getUsers(search,userStatus,userRole,pageIndex,pageSize);
                if (result.Status == false)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }      
        /// <summary>
        /// Get user width userId for customer
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet("{userid}")]
        public async Task <IActionResult> GetUser( int userid)
        {
            try
            {

                var result = await _userRepository.getUser(userid);
                return Ok(_mapper.Map<UsersDto>(result));
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
           
        }

        /// <summary>
        /// Get customer all
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        
        [HttpGet("get-customer-infor")]
        public async Task<IActionResult> GetCustomerAll([FromQuery] string? search, [FromQuery] int pageIndex= 1, [FromQuery] int pageSize=10 )
        {
            try
            {
                ValueReturn valueReturn = await _userRepository.getCustomerWithCriterial(search, pageIndex, pageSize);

                if (valueReturn.Status == true)
                {
                    return Ok(valueReturn.Data);

                }
                else { 
                        return BadRequest(valueReturn.Message);
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }


        /// <summary>
        /// Get Customer  With Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomerWithId(int id)
        {

            try
            {
                ValueReturn result = await _userRepository.getCustomerWithId(id); 
                if (result.Status == false ) {
                    return BadRequest(result.Message);
                }
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }


        /// <summary>
        /// Api Add User
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] UserCreateInput users)
        {
            try
            {
                if (users == null) return BadRequest("Hãy nhập thông tin đầy đủ");
                if (_userRepository.getUser(users.UserName)) return BadRequest("User da ton tai");
                var user = new Users()
                {
                    UserName = users.UserName,
                    Password = users.Password,

                };
                if (users.Gender != null)
                {
                    user.Gender = users.Gender;
                }
                return Ok(_userRepository.addUser(user));
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }

        }



        [HttpPost("add-user-for-admin")]
        public async Task<IActionResult> Add_User_Admin(UserCreateInputAdmin userInput)
        {
            try
            {
                ValueReturn result = await _userRepository.Add_User_Admin(userInput);
                if (result.Status == false )
                {
                    return BadRequest(result.Message);
                }
                else
                {
                    return Ok("Thêm thành công");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Api renew token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Api Delete User with userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        public async Task <IActionResult> DeleteUser(int userId)
        {
            try
            {
                ValueReturn result = await _userRepository.DeleteUser(userId);
                if (result.Status == true)
                {
                    return Ok("Xóa thành công");
                }
                else
                {
                    return BadRequest(result.Message);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            


        }
        /// <summary>
        /// api edit user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPut]
        public async Task<IActionResult> UpdateUser ([FromForm] UserInputModel user)
        {
            try

            {
                ValueReturn result = await _userRepository.Update(user);
                if (result.Status == true)
                {
                    return Ok("Cập nhật thành công");
                }
                else
                {
                    return BadRequest("Cập nhật thất bại. Có lỗi xảy ra ");
                }

                
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex);
            }
        }



        [HttpPut("for-admin")]
        public async Task<IActionResult> UpdateUserforAdmin([FromBody] UserInputModel user)
        {
            try
            {
                ValueReturn result = await _userRepository.Update(user);
                if (result.Status == true)
                {
                    return Ok("Cập nhật thành công");

                }
                else return BadRequest(result.Message);


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
