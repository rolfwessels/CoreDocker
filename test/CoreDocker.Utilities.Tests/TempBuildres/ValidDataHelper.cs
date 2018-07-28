﻿using System.Text.RegularExpressions;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace CoreDocker.Utilities.Tests.TempBuildres
{
    public static class ValidDataHelper
    {
        public static ISingleObjectBuilder<T> WithValidData<T>(this ISingleObjectBuilder<T> value)
        {
            return value.With(ValidData);
        }

        public static IListBuilder<T> WithValidData<T>(this IListBuilder<T> value)
        {
            return value.All().With(ValidData);
        }

        #region Private Methods

        private static T ValidData<T>(T value)
        {
            var project = value as Project;
            if (project != null)
                project.Name = GetRandom.String(20);

            var user = value as User;
            if (user != null)
            {
                user.Name = GetRandom.FirstName() + " " + GetRandom.LastName();
                user.Email = (Regex.Replace(user.Name.ToLower(),"[^a-z]","")+GetRandom.NumericString(3) + "@nomailmail.com").ToLower();
                user.HashedPassword = GetRandom.String(20);
                user.Roles.Add("Guest");
            }

            var userGrant = value as UserGrant;
            if (userGrant != null) userGrant.User = Builder<User>.CreateNew().Build().ToReference();
            return value;
        }

        #endregion
    }
}