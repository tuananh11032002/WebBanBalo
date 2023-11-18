using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebBanBalo.Data;
using WebBanBalo.HubService;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly AppSetting _appSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMessageService _messageService;
        public UserRepository(DataContext dataContext, IOptionsMonitor<AppSetting> optionsMonitor,
            IWebHostEnvironment webHostEnvironment, IMessageService messageService)
        {
            _messageService = messageService;
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
                    new Claim(ClaimTypes.Role, user.Role.ToString())

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

        public async Task<ValueReturn> getUser(LoginModel loginModel)
        {
            try
            {
                Users user = await _dataContext.Users.Where(p => p.UserName == loginModel.userName && p.Password == loginModel.password).FirstOrDefaultAsync();

                if (user == null)
                {
                    return new ValueReturn
                    {
                        Status = false,
                        Message = "Có lỗi gì đó với thông tin bạn nhập vào, chúng tôi không tiến hành đăng nhập cho bạn được"
                    };
                }
                else
                {
                    if (user.Status == UserStatus.Inactive)
                    {
                        return new ValueReturn
                        {
                            Status = false,
                            Message = "Tài khoản của bạn đang ở trạng thái Inactive, không thể đăng nhập"
                        };
                    }
                }
                return new ValueReturn
                {
                    Status = true,
                    Data = user,


                };

            }
            catch (Exception ex)
            {
                return new ValueReturn
                {
                    Status = false,
                    Message = ex.Message
                };
            }
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
            _dataContext.SaveChanges();

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

        public async Task<ValueReturn> getUsers(string? search, UserStatus? userStatus, UserRole? userRole, int pageIndex, int pageSize  )
        {

            try
            {
                var query = _dataContext.Users.AsQueryable();
                if (!search.IsNullOrEmpty())
                {
                    var searchLower = search.ToLower();

                    query = query.Where(p =>
                        (p.UserName != null && p.UserName.ToLower().Contains(searchLower)) ||
                        (p.HoTen != null && p.HoTen.ToLower().Contains(searchLower)) ||
                        (p.Email != null && p.Email.ToLower().Contains(searchLower)) 
                        
                    );
                }
                if (userRole != null)
                {
                    query=query.Where(p=>p.Role==userRole);
                }
                if (userStatus != null)
                {
                    query = query.Where(p => p.Status == userStatus);
                }
                var result =await query.Skip((pageIndex-1)*pageSize).Take(pageSize).
                    Select(p => new { 
                        Id = p.Id ,
                        UserName = p.UserName ,
                        DisplayName= p.HoTen,
                        Email =p.Email ,
                        Role = p.Role ,
                        Status = p.Status ,
                        Image = p.Image ,
                    }).ToListAsync();

                return new ValueReturn
                {
                    Status = true,
                    Data = new
                    {
                        totalUser = query.Count(),
                        userList = result
                    }
                };

            }
            catch (Exception ex)
            {
                return new ValueReturn
                {
                    Status = false,
                    Message = ex.Message
                };
            }
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
                    var userRole = user.Role;
                      if(userRole ==UserRole.Admin)
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
                            ICollection<Users> users = _dataContext.Users.Where(p => p.Role ==UserRole.Admin).ToList();
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

        public async Task<ValueReturn> DeleteUser(int userId)
        {
            try
            {
                Users users = await _dataContext.Users.FirstOrDefaultAsync(p=>p.Id==userId);
                if (users == null)
                {
                    return new ValueReturn
                    {
                        Status=false,
                        Message= "Không tìm thấy user"
                    };
                }
                else
                {
                    _dataContext.Users.Remove(users);
                    await _dataContext.SaveChangesAsync();
                    return new ValueReturn
                    {
                        Status = true,
                    };
                }
               

            }
            catch (Exception ex)
            {
                return new ValueReturn {
                    Status= false, 
                    Message=ex.Message

                };

            }
        }

        public async Task<ValueReturn> Update(UserInputModel user)
        {
            try
            {
                var image = user.Image;
                var userTemp = await _dataContext.Users.Where(p => p.Id == user.Id).FirstOrDefaultAsync();

                if(userTemp != null) {
                    if (image != null)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        string imagePath = Path.Combine("images", uniqueFileName);
                        var imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

                        using (var stream = new FileStream(imageFilePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        userTemp.Image = imagePath;
                    }
                    userTemp.HoTen = user.HoTen;
                    userTemp.Email = user.Email;
                    userTemp.Phone = user.Phone;
                    if (user.Gender != null)
                    {
                        userTemp.Gender = user.Gender.Value;

                    }
                    if (user.UserStatus != null)
                    {
                        userTemp.Status = user.UserStatus.Value;

                    }
                    await _dataContext.SaveChangesAsync();
                    if (userTemp.Status == UserStatus.Inactive)
                    {
                        await _messageService.LogoutNow(user.Id);
                    }
                    return new ValueReturn
                    {
                        Status = true,
                        Message = "Cập nhật thành công"
                    };
                }
                else
                {
                    return new ValueReturn
                    {
                        Status = false,
                        Message = "User không tồn tại"
                    };
                }
               
            }
            catch (Exception ex)
            {
              
                return new ValueReturn
                {
                    Status = false,
                    Message= ex.Message

                };
            }
        }

        public async Task<ValueReturn> getCustomerWithCriterial(string? search, int pageIndex, int pageSize)
        {
            try
            {

                var customer = _dataContext.Users
                        .Include(u => u.Orders)
                        .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    customer = customer.Where(u => u.HoTen.Contains(search) || u.UserName.Contains(search));
                }

                var listCustomer = await customer
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .AsQueryable()
                    .Select(p => new
                    {
                        Id = p.Id,
                        HoTen = p.HoTen,
                        Country = "Việt Nam",
                        Image = p.Image,
                        TotalSpent = p.Orders.Select(or => or.TotalAmount + or.FeeShip - or.Discount).Sum(),
                        Email = p.Email.IsNullOrEmpty() ? "Chưa có ": p.Email,
                        Order = p.Orders.Count
                    })
                    .ToListAsync();
              



                int totalCustomer = customer.Count();
                var result = new
                {
                    Customer = listCustomer,
                    TotalCustomer = totalCustomer

                };
                if (customer != null)
                {
                    return new ValueReturn { Status = true, Data = result };
                }
                else  return new ValueReturn { Status = false, Message="Kiểm tra lại pageIndex và pageSize"};
            }
            catch (Exception ex)
            {
                return new ValueReturn {Status=false, Message= ex.Message };

            }
        }

        public async Task<ValueReturn> getCustomerWithId(int id)
        {
            try
            {
                var data = await _dataContext.Users.Where(p => p.Id == id).Select(pc => new
                {
                    Image= pc.Image,
                    NumberOrder= pc.Orders.Where(p=>p.Done==true).Count(),
                    TotalSpent = pc.Orders.Select(or=>or.TotalAmount+ or.FeeShip-or.Discount).Sum(),
                    UserName= pc.UserName,
                    DisplayName = pc.HoTen,
                    Id= pc.Id,
                    Status = pc.Status,
                    Contact = pc.Phone.IsNullOrEmpty() ? "Chưa có" :pc.Phone,
                    Email= pc.Email.IsNullOrEmpty()?"Chưa có": pc.Email,
                    Role= pc.Role,
                    OrderList = pc.Orders.Where(p=>p.Done==true).Select(or=>new
                    {
                        Id= or.Id,
                        Date= or.FinishedAt,
                        Status= or.OrderStatusUpdates.OrderByDescending(pc => pc.UpdateTime).FirstOrDefault(),
                        Spent =  or.FeeShip+or.TotalAmount-or.Discount
                    }),

                }).FirstOrDefaultAsync();
                if(data == null)
                {
                    return new ValueReturn { Status = false, Message="Có vẻ Id bạn cung cấp không thuộc về sản phẩm nào hết" };


                }

                return new ValueReturn { Status= true, Data= data };

            }
            catch (Exception ex)
            {
                return new ValueReturn
                {
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ValueReturn> Add_User_Admin(UserCreateInputAdmin userInput)
        {
            try
            {
               
                var userExist=await _dataContext.Users.Where(p=>p.UserName==userInput.UserName).FirstOrDefaultAsync();
                if (userExist == null)
                {
                    Users users = new Users();
                    users.Role = userInput.UserRole;
                    users.UserName = userInput.UserName;
                    users.Email = userInput.Email;
                    users.Password = userInput.Password;
                    if (!userInput.hoTen.IsNullOrEmpty())
                    {
                        users.HoTen = userInput.hoTen;
                    }
                    else
                    {
                        users.HoTen = userInput.UserName;
                    }
                    _dataContext.Users.Add(users);
                    await _dataContext.SaveChangesAsync();
                    return new ValueReturn
                    {
                        Status = true,

                    };
                }
                else
                {
                    return new ValueReturn
                    {
                        Status = false,
                        Message = "UserName đã tồn tại "
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new ValueReturn { Status = false, Data = ex.Message };
            }
        }
    }
}
