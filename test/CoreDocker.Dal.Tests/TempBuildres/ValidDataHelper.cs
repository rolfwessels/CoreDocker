using System.Collections.Generic;
using FizzWare.NBuilder.Generator;
using MainSolutionTemplate.Dal.Models;

namespace FizzWare.NBuilder
{
    public static class ValidDataHelper
    {
        public static T WithValidData<T>(this T value)
        {
            var project = value as Project;
            if (project != null)
            {
                project.Name = GetRandom.String(20);
            }

            var user = value as User;
            if (user != null)
            {
                user.Name = GetRandom.String(20);
                user.Email = GetRandom.String(20)+"@nomailmail.com";
                user.HashedPassword = GetRandom.String(20);
                user.Roles.Add("Guest");
            }
            return value;
        }

        public static T Build<T>(this T value)
        {
            return value;
        }
    }
}