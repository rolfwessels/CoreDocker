using CoreDocker.Shared.Models.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Users
{
    public class RegisterType : InputObjectType<RegisterModel>
    {
        protected override void Configure(IInputObjectTypeDescriptor<RegisterModel> descriptor)
        {
            Name = "Register";
            descriptor.Field(d => d.Name).Description("The name of the user.");
            descriptor.Field(d => d.Email).Description("The email of the user.");
            descriptor.Field(d => d.Password).Description("The password of the user.");
        }
    }
}