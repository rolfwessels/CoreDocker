using System.Linq;
using System.Reflection;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsSpecification : ObjectGraphType<object>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ProjectsSpecification(ProjectCommonController projects)
        {
            var safe = new Safe(_log);
            var options = new GraphQlQueryOptions<ProjectCommonController, ProjectModel,Project >(projects);
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
                resolve: safe.Wrap( context => projects.GetById(context.GetArgument<string>("id")))
            ).RequirePermission(Activity.ReadProject);
            Field<ListGraphType<ProjectSpecification>>(
                "all",
                Description = "all projects",
                resolve: safe.Wrap(context => projects.Query(queryable => queryable))
            ).RequirePermission(Activity.ReadProject);
            Field<ListGraphType<ProjectSpecification>>(
                "recent",
                Description = "recent modified projects",
                new QueryArguments(
                    new QueryArgument<IntGraphType>
                    {
                        Name = "first",
                        Description = "id of the project"
                    }
                ),
                safe.Wrap(context => projects
                    .Query(queryable =>
                        queryable
                            .OrderByDescending(x => x.UpdateDate)
                            .Take(context.HasArgument("first") ? context.GetArgument<int>("first") : 100)
                    ))
            ).RequirePermission(Activity.ReadProject);
            Field<QueryResultSpecification>(
                "query",
                Description = "query the projects projects",
                options.GetArguments(),
                safe.Wrap(context => options.Query(context))
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
        "Field<ProjectsSpecification>(\"projects\",resolve: context => Task.FromResult(new object()));"
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
