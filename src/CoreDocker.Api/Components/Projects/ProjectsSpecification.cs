using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreDocker.Dal.Models;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsSpecification : ObjectGraphType<object>
    {
        public ProjectsSpecification(ProjectCommonController projects)
        {
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
                resolve: context => projects.GetById(context.GetArgument<string>("id"))
            );
            Field<ListGraphType<ProjectSpecification>>(
                "all",
                Description = "all projects",
                resolve: context => projects.Query(queryable => queryable)
            );
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
                context => projects
                    .Query(queryable =>
                        queryable
                            .OrderByDescending(x => x.UpdateDate)
                            .Take(context.HasArgument("first") ? context.GetArgument<int>("first") : 100)
                    )
            );
            Field<QueryResultSpecification>(
                "query",
                Description = "query the projects projects",
                options.GetArguments(),
                context => options.Query(context)
            );
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
