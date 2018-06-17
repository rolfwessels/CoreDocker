using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Components.Users
{
    public interface IUserManager : IBaseManager<User>
    {
        Task<User> Save(User user, string password);
        Task<User> GetUserByEmailAndPassword(string email, string password);
        Task<User> GetUserByEmail(string email);
        Task UpdateLastLoginDate(string email);
    }
}