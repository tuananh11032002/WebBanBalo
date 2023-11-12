using WebBanBalo.Dto;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface IUserRepository
    {
        bool addUser(Users user);
        TokenModel GenerateToken(Users user);
        Users getUser(LoginModel loginModel);
        bool getUser(string userName);
        List<Users> getUsers();
        Task<Users> getUser(int userid);
        bool IsHasFirstMessage(int id);
        bool DeleteUser(Users user);
        Task<bool> Update(UserInputModel user);
    }
}
