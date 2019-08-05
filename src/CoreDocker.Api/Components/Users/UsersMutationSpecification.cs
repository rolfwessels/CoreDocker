using System.Reflection;
using CoreDocker.Api.GraphQl;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models.Users;
using HotChocolate.Types;
using Serilog;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutationSpecification : ObjectType<UsersMutation>
    {
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
