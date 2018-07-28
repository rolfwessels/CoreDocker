using CoreDocker.Api.GraphQl;
using CoreDocker.Shared.Models.Users;
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
            Field(d => d.Email).Description("The email of the user.");
            Field(d => d.Roles).Description("The roles of the user.");
            Field(d => d.UpdateDate,true,typeof(OriginalDateGraphType)).Description("The date when the user was last updated.");
            Field(d => d.CreateDate, type: typeof(OriginalDateGraphType)).Description("The date when the user was created.");
        }
    }
}