using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.Mappers;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Serilog;

namespace CoreDocker.Api.Security
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IUserGrantLookup _userGrantLookup;
        private readonly IUserLookup _userLookup;

        #region Implementation of IPersistedGrantStore

        public PersistedGrantStore(IUserGrantLookup userGrantLookup, IUserLookup userLookup)
        {
            _userGrantLookup = userGrantLookup;
            _userLookup = userLookup;
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var userGrant = grant.ToGrant();
            var userById = await _userLookup.GetById(grant.SubjectId);
            if (userById != null) userGrant.User = userById.ToReference();
            await _userGrantLookup.Insert(userGrant);
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var byKey = await _userGrantLookup.GetByKey(key);
            return byKey.ToPersistanceGrant();
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var byKey = await _userGrantLookup.GetByUserId(subjectId);
            return byKey.Select(x => x.ToPersistanceGrant());
        }

        public async Task RemoveAsync(string key)
        {
            var byKey = await _userGrantLookup.GetByKey(key);
            if (byKey != null) await _userGrantLookup.Delete(byKey.Id);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            Log.Warning($"PersistedGrantStore:RemoveAllAsync For client {subjectId} {clientId} ");
            var byKey = await _userGrantLookup.GetByUserId(subjectId);
            foreach (var userGrant in byKey.Where(x => x.ClientId == clientId))
                await _userGrantLookup.Delete(userGrant.Id);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            Log.Warning($"PersistedGrantStore:RemoveAllAsync For client {subjectId} {clientId} ");
            var byKey = await _userGrantLookup.GetByUserId(subjectId);
            foreach (var userGrant in byKey.Where(x => x.ClientId == clientId && x.Type == type))
                await _userGrantLookup.Delete(userGrant.Id);
        }

        #endregion
    }
}
