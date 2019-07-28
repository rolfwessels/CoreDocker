using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectCreateUpdateSpecification : InputObjectGraphType<ProjectCreateUpdateModel>
    {
        public ProjectCreateUpdateSpecification()
        {
            Name = "ProjectCreateUpdate";
            Field(d => d.Name, true).Description("The name of the project.");
        }
    }
}
