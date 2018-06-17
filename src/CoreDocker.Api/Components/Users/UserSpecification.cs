using CoreDocker.Dal.Models;
using CoreDocker.Shared.Models;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UserSpecification : ObjectGraphType<UserModel>
    {
        public UserSpecification()
        {
            Name = "User";
            Field(d => d.Id).Description("The id of the user.");
            Field(d => d.Name).Description("The name of the user.");
            Field(d => d.Roles).Description("The roles of the user.");
            Field(d => d.UpdateDate, nullable: true).Description("The update date of the user.");
        }
    }
}