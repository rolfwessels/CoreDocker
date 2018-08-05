using CoreDocker.Shared.Models.Users;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class RegisterSpecification : InputObjectGraphType<RegisterModel>
    {
        public RegisterSpecification()
        {
            Name = "Register";
            Field(d => d.Name).Description("The name of the user.");
            Field(d => d.Email).Description("The email of the user.");
            Field(d => d.Password).Description("The password of the user.");
        }
    }
}