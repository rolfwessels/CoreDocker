using CoreDocker.Api.GraphQl;
using CoreDocker.Dal.Models.Auth;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutationType : ObjectType<ProjectsMutation>
    {
        protected override void Configure(IObjectTypeDescriptor<ProjectsMutation> descriptor)
        {
            Name = "ProjectsMutation";
            descriptor.Field(t => t.Create(default!))
                .Description("Add a project.")
                .RequirePermission(Activity.UpdateProject);

            descriptor.Field(t => t.Update(default!, default!))
                .Description("Update a project.")
                .RequirePermission(Activity.UpdateProject);

            descriptor.Field(t => t.Remove(default!))
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
      "Indexline": "ProjectsMutationType",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Field<ProjectsMutationType>(\"projects\", resolve: context => Task.BuildFromHttpContext(new object()));"
      ]
    }
] scaffolding */