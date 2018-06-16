using CoreDocker.Shared.Models;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutationSpecification : ObjectGraphType<object>
    {
        private const string Value = "project";
        public ProjectsMutationSpecification(ProjectCommonController projectManager)
        {
            Name = "MarketAppMutation";

            Field<ProjectSpecification>(
                "insert",
                Description = "add a project",
                new QueryArguments(
                    new QueryArgument<ProjectCreateUpdateSpecification> { Name = Value }
                ),
                context =>
                {
                    var project = context.GetArgument<ProjectCreateUpdateModel>(Name = Value);
                    return projectManager.Insert(project);
                });
            Field<ProjectSpecification>(
                "update",
                Description = "update a project",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" },
                    new QueryArgument<ProjectCreateUpdateSpecification> { Name = Value }
                ),
                context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    var project = context.GetArgument<ProjectCreateUpdateModel>(Name = Value);
                    return projectManager.Update(id, project);
                });

        }
    }
}