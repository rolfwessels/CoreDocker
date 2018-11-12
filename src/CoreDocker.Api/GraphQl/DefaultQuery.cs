using System.Threading.Tasks;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using CoreDocker.Dal.Models.Users;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl
{

    public class DefaultQuery : ObjectGraphType<object>
    {
        public DefaultQuery()
        {
            Name = "Query";
            Field<ProjectsQuerySpecification>("projects", resolve: context => Task.FromResult(new object()));
            Field<UsersQuerySpecification>("users", resolve: context => Task.FromResult(new object()));
            
        }
    }
}