using CoreDocker.Api.GraphQl;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectSpecification : ObjectGraphType<ProjectModel>
    {
        public ProjectSpecification()
        {
            Name = "Project";
            Field(d => d.Id).Description("The id of the project.");
            Field(d => d.Name).Description("The name of the project.");
            Field(d => d.UpdateDate, nullable: true, type: typeof(OriginalDateGraphType)).Description("The last updated date for the project.");
            Field(d => d.CreateDate,type: typeof(OriginalDateGraphType)).Description("The date when the project was created.");
        }
    }
}