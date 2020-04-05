using CoreDocker.Dal.Models.Projects;
using CoreDocker.Shared.Models.Projects;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectSpecification : ObjectType<Project>
    {
        protected override void Configure(IObjectTypeDescriptor<Project> descriptor)
        {
            Name = "Project";
            descriptor.Field(d => d.Id).Description("The id of the project.");
            descriptor.Field(d => d.Name).Description("The name of the project.");
            descriptor.Field(d => d.UpdateDate).Type<DateTimeType>()
                .Description("The last updated date for the project.");
            descriptor.Field(d => d.CreateDate).Type<NonNullType<DateTimeType>>()
                .Description("The date when the project was created.");
        }
    }
}