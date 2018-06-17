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
            Field(d => d.Name, true).Description("The name of the user.");
        }
    }
}