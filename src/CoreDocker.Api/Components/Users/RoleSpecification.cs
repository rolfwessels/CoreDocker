using CoreDocker.Shared.Models.Users;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class RoleSpecification : ObjectGraphType<RoleModel>
    {
        public RoleSpecification()
        {
            Name = "Role";
            Field(d => d.Name).Description("The name of the role.");
            Field(d => d.Activities).Description("List of allowed activities.");
        }
    }
}