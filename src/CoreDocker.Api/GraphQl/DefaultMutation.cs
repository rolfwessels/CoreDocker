using System.Threading.Tasks;
using CoreDocker.Api.Components.Projects;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl
{
    public class DefaultMutation : ObjectGraphType<object>
    {
     public DefaultMutation()
        {
            Name = "Mutation";
            Field<ProjectsMutationSpecification>("projects", resolve: context => Task.FromResult(new object()));
        }

    }
}