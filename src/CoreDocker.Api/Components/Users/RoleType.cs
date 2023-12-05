using CoreDocker.Shared.Models.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Users
{
    public class RoleType : ObjectType<RoleModel>
    {
        protected override void Configure(IObjectTypeDescriptor<RoleModel> descriptor)
        {
            Name = "Role";
            descriptor.Field(d => d.Name).Description("The name of the role.");
            descriptor.Field(d => d.Activities).Description("List of allowed activities.");
        }
    }
}