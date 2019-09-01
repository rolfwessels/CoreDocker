using CoreDocker.Api.GraphQl;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models.Projects;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutationSpecification : ObjectType<ProjectsMutation>
    {
        protected override void Configure(IObjectTypeDescriptor<ProjectsMutation> descriptor)
        {
            Name = "ProjectsMutation";
            descriptor.Field(t => t.Create(default(ProjectCreateUpdateModel)))
                .Type<CommandResultSpecification>()
                .Description("Add a project.")
                .RequirePermission(Activity.UpdateProject);

            descriptor.Field(t => t.Update(default(string), default(ProjectCreateUpdateModel)))
                .Type<CommandResultSpecification>()
                .Description("Update a project.")
                .RequirePermission(Activity.UpdateProject);

            descriptor.Field(t => t.Remove(default(string)))
                .Type<CommandResultSpecification>()
                .Description("Permanently remove a project.")
                .RequirePermission(Activity.DeleteProject);
        }
    }
}

/* scaffolding [
    
    {
      "FileName": "DefaultMutation.cs",
      "Indexline": "using CoreDocker.Api.Components.Projects;",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "using CoreDocker.Api.Components.Projects;"
      ]
    },
    {
      "FileName": "DefaultMutation.cs",
      "Indexline": "ProjectsMutationSpecification",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Field<ProjectsMutationSpecification>(\"projects\", resolve: context => Task.BuildFromHttpContext(new object()));"
      ]
    }
] scaffolding */