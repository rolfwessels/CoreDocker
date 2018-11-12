using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Components.Users
{
    public interface IUserGrantManager : IBaseManager<UserGrant>
    {
        Task<UserGrant> GetByKey(string key);
        Task<List<UserGrant>> GetByUserId(string userId);
        Task Insert(UserGrant userGrant);
        Task Delete(string byKeyId);
    }
}