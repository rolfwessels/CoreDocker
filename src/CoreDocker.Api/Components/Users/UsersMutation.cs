﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Persistence;
using CoreDocker.Shared.Models.Users;
using HotChocolate;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutation
    {
        private readonly ICommander _commander;
        private readonly IIdGenerator _generator;

        public UsersMutation(ICommander commander, IIdGenerator generator)
        {
            _commander = commander;
            _generator = generator;
        }

        public Task<CommandResult> Create(
            [GraphQLType(typeof(NonNullType<UserCreateUpdateType>))]
            UserCreateUpdateModel user)
        {
            return _commander.Execute(UserCreate.Request.From(_generator.NewId, user.Name, user.Email,
                user.Password, user.Roles), CancellationToken.None);
        }

        public Task<CommandResult> Update(
            [GraphQLNonNullType] string id,
            [GraphQLType(typeof(NonNullType<UserCreateUpdateType>))]
            UserCreateUpdateModel user)
        {
            return _commander.Execute(UserUpdate.Request.From(id, user.Name, user.Password, user.Roles,
                user.Email), CancellationToken.None);
        }

        public Task<CommandResult> Remove([GraphQLNonNullType] string id)
        {
            return _commander.Execute(UserRemove.Request.From(id), CancellationToken.None);
        }

        public Task<CommandResult> Register([GraphQLNonNullType] [GraphQLType(typeof(RegisterType))]
            RegisterModel user)
        {
            return _commander.Execute(UserCreate.Request.From(_generator.NewId, user.Name, user.Email,
                user.Password, new List<string>() {RoleManager.Guest.Name}), CancellationToken.None);
        }
    }
}