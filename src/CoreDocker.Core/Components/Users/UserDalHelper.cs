using System;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Components.Users
{
    public  static class UserDalHelper
    {

        public static bool IsPassword(this User user, string password)
        {
            return PasswordHash.ValidatePassword(password, user.HashedPassword);
        }

        public static string SetPassword(this User user, string password)
        {
            return user.HashedPassword = UserDalHelper.SetPassword(password);
        }

        public static string SetPassword(string password)
        {
            return PasswordHash.CreateHash(password ?? Guid.NewGuid().ToString());
        }
    }
}