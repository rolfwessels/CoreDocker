using System;
using System.Collections.Generic;
using System.Text;
using Bumbershoot.Utilities.Helpers;

namespace CoreDocker.Core.Components.Users
{
    public static class GravatarHelper
    {
        public static string BuildUrl(string? email)
        {
            var md5Hash = Md5Hash(email.OrEmpty().ToLower().Trim());
            return $"https://www.gravatar.com/avatar/{md5Hash}?d=robohash";
        }

        private static string Md5Hash(string value)
        {

            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(value);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}
