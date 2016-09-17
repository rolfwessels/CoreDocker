using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generator;
using CoreDocker.Dal.Models;
using CoreDocker.Shared.Models;

namespace FizzWare.NBuilder
{
    public static class ValidDataHelper
    {
        public static IList<T> All<T>(this IList<T> value)
        {
            return value;
        }

        public static IList<T> WithValidModelData<T>(this IList<T> value1)
        {
            foreach (var value in value1)
            {
                WithValidModelData(value);
            }
            
            return value1;
        }

        public static T WithValidModelData<T>(this T value)
        {
            var project = value as ProjectCreateUpdateModel;
            if (project != null)
            {
                project.Name = GetRandom.String(20);
            }

            var user = value as UserCreateUpdateModel;
            if (user != null)
            {
                user.Name = GetRandom.String(20);
                user.Email = GetRandom.String(20) + "@nomailmail.com";
            }
            return value;
        }

        public static IList<T> WithValidData<T>(this IList<T> value)
        {
            return value;
        }

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
        public static T With<T>(this T value , Action<T> apply )
        {
            apply(value);
            return value;
        }
    }
}