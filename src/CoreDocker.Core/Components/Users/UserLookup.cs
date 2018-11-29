using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Logging;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using CoreDocker.Utilities.Helpers;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Components.Users
{
    public class UserLookup : BaseLookup<User>, IUserLookup
    {
        private readonly ILogger<UserLookup> _log;

        public UserLookup(BaseManagerArguments baseManagerArguments, ILogger<UserLookup> logger) : base(
            baseManagerArguments, logger)
        {
            _log = logger;
        }

        #region Overrides of BaseLookup<User>

        protected override IRepository<User> Repository => _generalUnitOfWork.Users;

        #endregion

        #region IUserLookup Members

        public Task<PagedList<User>> GetPagedUsers(UserPagedLookupOptions options)
        {
            return Task.Run(() =>
            {
                var query = _generalUnitOfWork.Users.Query();
                if (!string.IsNullOrEmpty(options.Search))
                {
                    query = query.Where(x =>
                        x.Id.ToLower().Contains(options.Search.ToLower()) ||
                        x.Email.ToLower().Contains(options.Search.ToLower()) ||
                        x.Name.ToLower().Contains(options.Search.ToLower()));
                }
                if (options.Sort != null)
                {
                    switch (options.Sort)
                    {
                        case UserPagedLookupOptions.SortOptions.Name:
                            query = query.OrderBy(x => x.Name);
                            break;
                        case UserPagedLookupOptions.SortOptions.Recent:
                            query = query.OrderByDescending(x => x.UpdateDate);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return new PagedList<User>(query, options);
            });

        }

        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            var user = await GetUserByEmail(email);
            if (user?.HashedPassword != null)
            {
                if (!PasswordHash.ValidatePassword(password, user.HashedPassword))
                {
                    user = null;
                    _log.Info($"Invalid password for user '{email}'");
                }
            }
            else
            {
                _log.Info($"Invalid user '{email}'");
            }

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            return await _generalUnitOfWork.Users.FindOne(x => x.Email == email.ToLower());
        }

        public async Task UpdateLastLoginDate(string email)
        {
            var userFound = await GetUserByEmail(email);
            if (userFound == null) throw new ArgumentException("Invalid email address.");
            userFound.LastLoginDate = DateTime.Now;
//            await Update(userFound);
        }

        #endregion

      
    }
}