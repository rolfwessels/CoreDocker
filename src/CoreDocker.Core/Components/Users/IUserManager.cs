using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Components.Users
{
    public interface IUserManager : IBaseManager<User>
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByEmailAndPassword(string contextUserName, string contextPassword);
    }
}