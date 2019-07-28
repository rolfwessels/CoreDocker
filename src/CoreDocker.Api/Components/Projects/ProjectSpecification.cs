using CoreDocker.Dal.Models.Projects;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectSpecification : ObjectGraphType<Project>
    {
        public ProjectSpecification()
        {
            Name = "Project";
            Field(d => d.Id).Description("The id of the project.");
            Field(d => d.Name).Description("The name of the project.");
            Field(d => d.UpdateDate, true, typeof(DateTimeGraphType))
                .Description("The last updated date for the project.");
            Field(d => d.CreateDate, type: typeof(DateTimeGraphType))
                .Description("The date when the project was created.");
        }
    }
}
