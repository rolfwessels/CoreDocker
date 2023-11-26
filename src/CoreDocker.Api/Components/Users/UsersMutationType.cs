using CoreDocker.Api.GraphQl;
using CoreDocker.Dal.Models.Auth;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutationType : ObjectType<UsersMutation>
    {
        protected override void Configure(IObjectTypeDescriptor<UsersMutation> descriptor)
        {
            Name = "UsersMutation";
            descriptor.Field(x => x.Create(default!))
                .Description("Add a user.")
                .RequirePermission(Activity.UpdateUsers);

            descriptor.Field(x => x.Update(default!, default!))
                .Description("Update a user.")
                .RequirePermission(Activity.UpdateUsers);

            descriptor.Field(x => x.Remove(default!))
                .Description("Permanently remove a user.")
                .RequirePermission(Activity.DeleteUser);

            descriptor.Field(x => x.Register(default!))
                .Description("Register a new user.");
        }
    }
}