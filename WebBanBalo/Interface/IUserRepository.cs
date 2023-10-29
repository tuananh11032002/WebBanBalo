using WebBanBalo.Model;

namespace WebBanBalo.Interface
{
    public interface IUserRepository
    {
        bool addUser(Users user);
        TokenModel GenerateToken(Users user);
        Users getUser(LoginModel loginModel);
        bool getUser(string userName);
        List<Users> getUsers();
        Users getUser(int userid);
        bool IsHasFirstMessage(int id);
    }
}
