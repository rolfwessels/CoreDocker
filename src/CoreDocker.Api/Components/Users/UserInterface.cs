using CoreDocker.Dal.Models;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UserInterface : ObjectGraphType<User>
    {
        public UserInterface()
        {
            Name = "User";
            Field(d => d.Id).Description("The id of the user.");
            Field(d => d.Name, nullable: true).Description("The name of the user.");
            Field(d => d.Email, nullable: true).Description("The email of the user.");
//            Field(d => d.DefaultProject, nullable: true).Description("DefaultProject.");

            Field(d => d.UpdateDate, nullable: true).Description("The update date of the user.");
        }
    }
}