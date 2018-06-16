using CoreDocker.Dal.Models;
using CoreDocker.Shared.Models;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectSpecification : ObjectGraphType<ProjectModel>
    {
        public ProjectSpecification()
        {
            Name = "Project";
            Field(d => d.Id).Description("The id of the project.");
            Field(d => d.Name, nullable: true).Description("The name of the project.");
            Field(d => d.UpdateDate, nullable: true).Description("The update date of the project.");
        }
    }
}