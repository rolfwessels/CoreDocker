using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models.Users;

namespace CoreDocker.Shared.Interfaces.Shared
{
    public interface IUserControllerActions : ICrudController<UserModel, UserCreateUpdateModel>
    {
        Task<UserModel> Register(RegisterModel user);
//        Task<bool> ForgotPassword(string email);
        Task<UserModel> WhoAmI();
        Task<List<RoleModel>> Roles();
    }
}