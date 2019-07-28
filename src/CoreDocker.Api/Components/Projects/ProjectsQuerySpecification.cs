using System.Linq;
using System.Reflection;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;
using Serilog;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsQuerySpecification : ObjectGraphType<object>
    {

        public ProjectsQuerySpecification(IProjectLookup projects)
        {
            var safe = new Safe(Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType));
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
                "list",
                Description = "all projects",
                options.GetArguments(),
                resolve: safe.Wrap(context => options.Query(context))
            ).RequirePermission(Activity.ReadProject);

            Field<PagedListGraphType<Project, ProjectSpecification>>(
                "paged",
                Description = "all projects paged",
                options.GetArguments(),
                resolve: safe.Wrap(context => options.Paged(context))
            ).RequirePermission(Activity.ReadProject);
        }

        //PagedList<TDal>
    }

    public class PagedListGraphType<TDal, TGt> : ObjectGraphType<PagedList<TDal>> where TGt : IGraphType
    {
        public PagedListGraphType()
        {
            Name = $"{typeof(TDal).Name}PagedList";
            Field<ListGraphType<TGt>>(
                "items",
                Description = "All projects paged.",
                new QueryArguments(), context => context.Source.Items
            ).RequirePermission(Activity.ReadProject);
            Field<IntGraphType>(
                "count",
                Description = "The total count.",
                new QueryArguments(), context => context.Source.Count
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
