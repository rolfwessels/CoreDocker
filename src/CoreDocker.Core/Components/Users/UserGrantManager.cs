using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Components.Users
{
    public class UserGrantManager : BaseManager<UserGrant>, IUserGrantManager
    {
        public UserGrantManager(BaseManagerArguments baseManagerArguments, ILogger<UserGrantManager> logger) : base(
            baseManagerArguments, logger)
        {
        }

        #region Overrides of BaseManager<UserGrant>

        protected override IRepository<UserGrant> Repository => _generalUnitOfWork.UserGrants;

        #endregion

        #region Implementation of IUserGrantManager

        public Task<UserGrant> GetByKey(string key)
        {
            return Repository.FindOne(x => x.Key == key);
        }

        public Task<List<UserGrant>> GetByUserId(string userId)
        {
            return Repository.Find(x => x.User.Id == userId);
        }

        public Task Insert(UserGrant userGrant)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(string byKeyId)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}