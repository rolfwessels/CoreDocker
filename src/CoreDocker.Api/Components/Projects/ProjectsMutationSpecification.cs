using System.Reflection;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Authorization;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutationSpecification : ObjectGraphType<object>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string Value = "project";
        public ProjectsMutationSpecification(ProjectCommonController projectManager)
        {
            Name = "ProjectsMutation";
            var safe = new Safe(_log);
            
            this.RequireAuthorization();
            Field<ProjectSpecification>(
                "insert",
                Description = "add a project",
                new QueryArguments(
                    new QueryArgument<ProjectCreateUpdateSpecification> { Name = Value }
                ),
                safe.Wrap(context =>
                {
                    var project = context.GetArgument<ProjectCreateUpdateModel>(Name = Value);
                    return projectManager.Insert(project);
                })).RequirePermission(Activity.UpdateProject);

            Field<ProjectSpecification>(
                "update",
                Description = "update a project",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" },
                    new QueryArgument<ProjectCreateUpdateSpecification> { Name = Value }
                ),
                safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    var project = context.GetArgument<ProjectCreateUpdateModel>(Name = Value);
                    return projectManager.Update(id, project);
                })).RequirePermission(Activity.UpdateProject);

            Field<BooleanGraphType>(
                "delete",
                Description = "permanently remove a project",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }
                ),
                safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    return projectManager.Delete(id);
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
