using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using Serilog;

namespace CoreDocker.Core.Components.Users
{
    public class UserLookup : BaseLookup<User>, IUserLookup
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public UserLookup(IRepository<User> users)
        {
            Repository = users;
        }

        protected override IRepository<User> Repository { get; }

        public Task<PagedList<User>> GetPagedUsers(UserPagedLookupOptions options)
        {
            return Task.Run(() =>
            {
                var query = Repository.Query();
                if (!string.IsNullOrEmpty(options.Search))
                {
                    query = query.Where(x =>
                        x.Id.ToLower().Contains(options.Search.ToLower()) ||
                        x.Email.OrEmpty().ToLower().Contains(options.Search.ToLower()) ||
                        x.Name.OrEmpty().ToLower().Contains(options.Search.ToLower()));
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

        public async Task<User?> GetUserByEmailAndPassword(string? email, string password)
        {
            var user = await GetUserByEmail(email);
            if (user?.HashedPassword != null)
            {
                if (!PasswordHash.ValidatePassword(password, user.HashedPassword))
                {
                    user = null;
                    _log.Warning("Invalid password for user '{email}'", email);
                }
            }
            else
            {
                _log.Warning("Invalid user '{email}'", email);
            }

            return user;
        }

        public async Task<User?> GetUserByEmail(string? email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await Repository.FindOne(x => x.Email == email.ToLower());
        }

        public async Task UpdateLastLoginDate(string email)
        {
            var updateOne = await Repository.UpdateOne(x => x.Email == email.ToLower(),
                calls => calls.Set(x => x.LastLoginDate, DateTime.Now));
            if (updateOne == 0)
            {
                throw new ArgumentException("Invalid email address.");
            }
        }
    }
}