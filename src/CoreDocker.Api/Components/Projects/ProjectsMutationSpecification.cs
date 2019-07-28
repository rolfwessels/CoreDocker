using System;
using System.Reflection;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;
using Serilog;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutationSpecification : ObjectGraphType<object>
    {
        private const string Value = "project";

        public ProjectsMutationSpecification(ICommander commander)
        {
            Name = "ProjectsMutation";
            var safe = new Safe(Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType));

            this.RequireAuthorization();
            Field<CommandResultSpecification>(
                "create",
                Description = "Add a project.",
                new QueryArguments(
                    new QueryArgument<ProjectCreateUpdateSpecification> {Name = Value}
                ),
                safe.Wrap(context =>
                {
                    var project = context.GetArgument<ProjectCreateUpdateModel>(Name = Value);
                    return commander.Execute(ProjectCreate.Request.From(commander.NewId, project.Name));
                })).RequirePermission(Activity.UpdateProject);

            Field<CommandResultSpecification>(
                "update",
                Description = "Update a project.",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"},
                    new QueryArgument<ProjectCreateUpdateSpecification> {Name = Value}
                ),
                safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    var project = context.GetArgument<ProjectCreateUpdateModel>(Name = Value);
                    return commander.Execute(ProjectUpdate.Request.From(id, project.Name));
                })).RequirePermission(Activity.UpdateProject);

            Field<CommandResultSpecification>(
                "remove",
                Description = "Permanently remove a project.",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"}
                ),
                safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    return commander.Execute(ProjectRemove.Request.From(id));
                })).RequirePermission(Activity.DeleteProject);
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
