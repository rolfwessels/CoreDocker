﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public UserLookup(BaseManagerArguments baseManagerArguments) : base(
            baseManagerArguments)
        {
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
                    query = query.Where(x =>
                        x.Id.ToLower().Contains(options.Search.ToLower()) ||
                        x.Email.ToLower().Contains(options.Search.ToLower()) ||
                        x.Name.ToLower().Contains(options.Search.ToLower()));

                if (options.Sort != null)
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
                    _log.Information($"Invalid password for user '{email}'");
                }
            }
            else
            {
                _log.Information($"Invalid user '{email}'");
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