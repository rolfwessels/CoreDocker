using System.Threading.Tasks;
using CoreDocker.Api.GraphQl;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models.Projects;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Projects
{

    public class ProjectsMutation
    {
        private readonly ICommander _commander;

        public ProjectsMutation(ICommander commander)
        {
            _commander = commander;
        }

        public Task<CommandResult> Create(ProjectCreateUpdateModel project)
        {
            return _commander.Execute(ProjectCreate.Request.From(_commander.NewId, project.Name));
        }

        public Task<CommandResult> Update(string id, ProjectCreateUpdateModel project)
        {
            return _commander.Execute(ProjectUpdate.Request.From(id, project.Name));
        }

        public Task<CommandResult> Remove(string id)
        {
            return _commander.Execute(ProjectRemove.Request.From(id));
        }
    }

    public class ProjectsMutationSpecification : ObjectType<ProjectsMutation>
    {
        protected override void Configure(IObjectTypeDescriptor<ProjectsMutation> descriptor )
        {
            Name = "ProjectsMutation";
            descriptor.Field(t => t.Create(default(ProjectCreateUpdateModel)))
                .Type<CommandResultSpecification>()
                .Description("Add a project.")
                .Argument("project", r => r.Type<NonNullType<ProjectCreateUpdateSpecification>>().Description("Project to add"))
                .RequirePermission(Activity.UpdateProject);

            descriptor.Field(t => t.Update(default(string),default(ProjectCreateUpdateModel)))
                .Type<CommandResultSpecification>()
                .Description("Update a project.")
                .Argument("id", r => r.Type<NonNullType<StringType>>().Description("Project id to update"))
                .Argument("project", r => r.Type<ProjectCreateUpdateSpecification>().Description("Project details to update"))
                .Resolver(x => x.Parent<ProjectsMutation>().Update(x.Argument<string>("id"), x.Argument<ProjectCreateUpdateModel>("project")))
                .RequirePermission(Activity.UpdateProject);

            descriptor.Field(t => t.Remove(default(string)))
                    .Type<CommandResultSpecification>()
                    .Description("Permanently remove a project.")
                    .Argument("id", r => r.Type<NonNullType<StringType>>().Description("Project id to remove"))
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
