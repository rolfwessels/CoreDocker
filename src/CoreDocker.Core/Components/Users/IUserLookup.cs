using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Components.Users
{
    public interface IUserLookup : IBaseLookup<User>
    {   
        Task<PagedList<User>> GetPagedUsers(UserPagedLookupOptions options);
        Task<User?> GetUserByEmail(string? email);
        Task<User?> GetUserByEmailAndPassword(string? contextUserName, string contextPassword);
    }
}