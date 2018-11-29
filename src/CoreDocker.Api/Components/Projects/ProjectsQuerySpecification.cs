using System.Linq;
using System.Reflection;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsQuerySpecification : ObjectGraphType<object>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProjectsQuerySpecification(IProjectLookup projects)
        {
            var safe = new Safe(_log);
            var options = new GraphQlQueryOptions<Project, ProjectPagedLookupOptions>(projects.GetPaged);
            Name = "Projects";

            Field<ProjectSpecification>(
                "byId",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "id",
                        Description = "id of the project"
                    }
                ),
                resolve: safe.Wrap(context => projects.GetById(context.GetArgument<string>("id")))
            ).RequirePermission(Activity.ReadProject);

            Field<ListGraphType<ProjectSpecification>>(
                "all",
                Description = "all projects",
                resolve: safe.Wrap(context => options.Query(context))
            ).RequirePermission(Activity.ReadProject);

            Field<ListGraphType<ProjectSpecification>>(
                "paged",
                Description = "all projects paged",
                resolve: safe.Wrap(context => options.Paged(context))
            ).RequirePermission(Activity.ReadProject);

            Field<ListGraphType<ProjectSpecification>>(
                "recent",
                Description = "recent modified projects",
                options.GetArguments(),
                safe.Wrap(context => safe.Wrap(x => options.Query(context,new ProjectPagedLookupOptions() {Sort = ProjectPagedLookupOptions.SortOptions.Recent})))
            ).RequirePermission(Activity.ReadProject);
        }
    }
}

/* scaffolding [
    {
      "FileName": "DefaultQuery.cs",
      "Indexline": "using CoreDocker.Api.Components.Projects;",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "using CoreDocker.Api.Components.Projects;"
      ]
    },
    {
      "FileName": "DefaultQuery.cs",
      "Indexline": "Field<ProjectsSpecification>",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Field<ProjectsSpecification>(\"projects\",resolve: context => Task.BuildFromHttpContext(new object()));"
      ]
    },
    {
      "FileName": "IocApi.cs",
      "Indexline": "\/*project*\/",
      "InsertAbove": true,
      "InsertInline": false,
      "Lines": [
            "/*project*\/",
            "builder.RegisterType<ProjectSpecification>().SingleInstance();",
            "builder.RegisterType<ProjectsSpecification>().SingleInstance();",
            "builder.RegisterType<ProjectCreateUpdateSpecification>().SingleInstance();",
            "builder.RegisterType<ProjectsMutationSpecification>().SingleInstance();",
            "",
      ]
    }
       

         
] scaffolding */