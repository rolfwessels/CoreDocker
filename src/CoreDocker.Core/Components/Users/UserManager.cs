using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.Logging;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using CoreDocker.Utilities.Helpers;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Components.Users
{
    public class UserManager : BaseManager<User>, IUserManager
    {
        private readonly ILogger<UserManager> _log;

        public UserManager(BaseManagerArguments baseManagerArguments, ILogger<UserManager> logger) : base(
            baseManagerArguments, logger)
        {
            _log = logger;
        }

        #region Overrides of BaseManager<User>

        protected override IRepository<User> Repository => _generalUnitOfWork.Users;

        #endregion

        #region IUserManager Members


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