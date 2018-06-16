using System.Threading.Tasks;
using CoreDocker.Api.Components.Projects;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl
{
    // more https://github.com/enesdalgaa/MarketApp/blob/master/MarketApp.WebService/Schemas/MarketAppQuery.cs
    public class DefaultQuery : ObjectGraphType<object>
    {
        public DefaultQuery()
        {
            Name = "Query";
            Field<ProjectsSpecification>("projects",resolve: context => Task.FromResult(new object()));
        }

       
    }
}