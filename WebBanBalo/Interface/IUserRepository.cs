using WebBanBalo.Dto;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface IUserRepository
    {
        bool addUser(Users user);
        TokenModel GenerateToken(Users user);
        Task<ValueReturn> getUser(LoginModel loginModel);
        bool getUser(string userName);
        Task <ValueReturn> getUsers(string? search,UserStatus? userStatus,UserRole? userRole, int pageIndex, int pageSize);
        Task<Users> getUser(int userid);
        bool IsHasFirstMessage(int id);
        Task<ValueReturn> DeleteUser(int userId);
        Task<ValueReturn> Update(UserInputModel user);
        Task<ValueReturn> getCustomerWithCriterial(string? search, int pageIndex, int pageSize);
        Task<ValueReturn> getCustomerWithId(int id);
        Task<ValueReturn> Add_User_Admin(UserCreateInputAdmin userInput);
        Task<ValueReturn> ChangePassword(ChangePasswordModel model, int userId);
    }
}
