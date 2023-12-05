using System.Security.Cryptography;
using System.Text;
using Bumbershoot.Utilities.Helpers;

namespace CoreDocker.Core.Components.Users
{
    public static class GravatarHelper
    {
        public static string BuildUrl(string? email)
        {
            var md5Hash = Md5Hash((email??"").ToLower().Trim());
            return $"https://www.gravatar.com/avatar/{md5Hash}?d=robohash";
        }

        private static string Md5Hash(string value)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
    }
}