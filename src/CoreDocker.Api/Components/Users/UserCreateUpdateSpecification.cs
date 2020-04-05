using CoreDocker.Shared.Models.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UserCreateUpdateSpecification : InputObjectType<UserCreateUpdateModel>
    {
        protected override void Configure(IInputObjectTypeDescriptor<UserCreateUpdateModel> descriptor)
        {
            Name = "UserCreateUpdate";
            descriptor.Field(d => d.Name).Description("The name of the user.");
            descriptor.Field(d => d.Email).Description("The email of the user.");
            descriptor.Field(d => d.Roles).Description("The users roles.");
            descriptor.Field(d => d.Password).Description("The password of the user.");
        }
    }
}