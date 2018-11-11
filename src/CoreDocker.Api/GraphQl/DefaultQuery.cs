using System.Threading.Tasks;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl
{
    // more https://github.com/enesdalgaa/MarketApp/blob/master/MarketApp.WebService/Schemas/MarketAppQuery.cs
    public class DefaultQuery : ObjectGraphType<object>
    {
        public DefaultQuery()
        {
            Name = "Query";
            Field<ProjectsSpecification>("projects", resolve: context => Task.FromResult(new object()));
            Field<UsersSpecification>("users", resolve: context => Task.FromResult(new object()));
        }
    }
}