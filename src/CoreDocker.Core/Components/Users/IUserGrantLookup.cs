using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Components.Users
{
    public interface IUserGrantLookup : IBaseLookup<UserGrant>
    {
        Task<UserGrant> GetByKey(string key);
        Task<List<UserGrant>> GetByUserId(string userId);
        Task Insert(UserGrant userGrant);
        Task Delete(string id);
    }
}