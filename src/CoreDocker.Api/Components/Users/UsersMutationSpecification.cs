using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.GraphQl;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models.Users;
using HotChocolate.Types;
using Serilog;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutation
    {
        private readonly ICommander _commander;

        public UsersMutation(ICommander commander)
        {
            _commander = commander;
        }

        public Task<CommandResult> Create(UserCreateUpdateModel user)
        {
            return _commander.Execute(UserCreate.Request.From(_commander.NewId, user.Name, user.Email,
                user.Password, user.Roles));
        }

        public Task<CommandResult> Update(string id,UserCreateUpdateModel user)
        {
            return _commander.Execute(UserUpdate.Request.From(id, user.Name, user.Password, user.Roles,
                user.Email));
        }

        public Task<CommandResult> Remove(string id)
        {
            return _commander.Execute(UserRemove.Request.From(id));
        }

        public Task<CommandResult> Register(RegisterModel user)
        {
            return _commander.Execute(UserCreate.Request.From(_commander.NewId, user.Name, user.Email,
                user.Password, new List<string>() {RoleManager.Guest.Name}));
        }
    }

    public class UsersMutationSpecification : ObjectType<UsersMutation>
    {
        private const string Value = "user";

        protected override void Configure(IObjectTypeDescriptor<UsersMutation> descriptor )
        {
            Name = "UsersMutation";

            descriptor.Field(x=>x.Create(default(UserCreateUpdateModel)))
                .Description( "Add a user.")
                .RequirePermission(Activity.UpdateUsers);

            descriptor.Field(x => x.Update(default(string), default(UserCreateUpdateModel)))
                .Description( "Update a user.")
                .RequirePermission(Activity.UpdateUsers);

            descriptor.Field(x => x.Remove(default(string)))
                .Description("Permanently remove a user.")
                .RequirePermission(Activity.DeleteUser);

            descriptor.Field(x => x.Register(default(RegisterModel)))
                .Description("Register a new user.");
        }
    }
}
