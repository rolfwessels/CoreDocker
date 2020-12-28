using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CoreDocker.Api.Mappers;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Models.Users;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Serilog;

namespace CoreDocker.Api.Security
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

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

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var fromDbByFilter = await FromDbByFilter(filter);
            return fromDbByFilter.Select(x => x.ToPersistanceGrant());
        }

        public async Task RemoveAsync(string key)
        {
            var byKey = await _userGrantLookup.GetByKey(key);
            if (byKey != null) await _userGrantLookup.Delete(byKey.Id);
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            _log.Warning($"PersistedGrantStore:RemoveAllAsync For client {filter.SubjectId} {filter.ClientId} ");
            var fromDbByFilter = await FromDbByFilter(filter);
            foreach (var userGrant in fromDbByFilter)
                await _userGrantLookup.Delete(userGrant.Id);
        }

        private async Task<IEnumerable<UserGrant>> FromDbByFilter(PersistedGrantFilter filter)
        {
            return (await _userGrantLookup.GetByUserId(filter.SubjectId)).Where(x => x.ClientId == filter.ClientId);
        }


        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _log.Warning($"PersistedGrantStore:RemoveAllAsync For client {subjectId} {clientId} ");
            var byKey = await _userGrantLookup.GetByUserId(subjectId);
            foreach (var userGrant in byKey.Where(x => x.ClientId == clientId && x.Type == type))
                await _userGrantLookup.Delete(userGrant.Id);
        }

        #endregion
    }
}