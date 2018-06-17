using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.Mappers;
using CoreDocker.Core.Components.Users;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using log4net;

namespace CoreDocker.Api.Security
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUserGrantManager _userGrantManager;

        #region Implementation of IPersistedGrantStore

        public PersistedGrantStore(IUserGrantManager userGrantManager)
        {
            _userGrantManager = userGrantManager;
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            var userGrant = grant.ToGrant();
            return _userGrantManager.Insert(userGrant);
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var byKey = await _userGrantManager.GetByKey(key);
            return byKey.ToPersistanceGrant();
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var byKey = await _userGrantManager.GetByUserId(subjectId);
            return byKey.Select(x => x.ToPersistanceGrant());
        }

        public async Task RemoveAsync(string key)
        {
            var byKey = await _userGrantManager.GetByKey(key);
            if (byKey != null) await _userGrantManager.Delete(byKey.Id);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            _log.Warn($"PersistedGrantStore:RemoveAllAsync For client {subjectId} {clientId} ");
            var byKey = await _userGrantManager.GetByUserId(subjectId);
            foreach (var userGrant in byKey.Where(x=>x.ClientId == clientId))
            {
                await _userGrantManager.Delete(userGrant.Id);
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _log.Warn($"PersistedGrantStore:RemoveAllAsync For client {subjectId} {clientId} ");
            var byKey = await _userGrantManager.GetByUserId(subjectId);
            foreach (var userGrant in byKey.Where(x => x.ClientId == clientId && x.Type == type))
            {
                await _userGrantManager.Delete(userGrant.Id);
            }
        }

        #endregion
    }
}