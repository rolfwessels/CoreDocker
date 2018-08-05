using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UserCreateUpdateSpecification : InputObjectGraphType<UserCreateUpdateModel>
    {
        public UserCreateUpdateSpecification()
        {
            Name = "UserCreateUpdate";
            Field(d => d.Name).Description("The name of the user.");
            Field(d => d.Email).Description("The email of the user.");
            Field(d => d.Roles, true).Description("The users roles.");
            Field(d => d.Password, true).Description("The password of the user.");
        }
    }
}