using System;

namespace FizzWare.NBuilder.Generator
{
    public class GetRandom
    {
        private static readonly Random _r = new Random();

        public static string String(int v)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = "";
            for (var i = 0; i < v; i++)
            {
                result += chars[_r.Next(chars.Length)];
            }
            return result.ToLower();
        }

        public static string Email()
        {
            return String(20).ToLower() + "@" + String(5).ToLower() + ".com";
        }
    }
}