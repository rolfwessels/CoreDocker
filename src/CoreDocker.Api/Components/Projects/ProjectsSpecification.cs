using System.Linq;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsSpecification : ObjectGraphType<object>
    {
        public ProjectsSpecification(ProjectCommonController projects)
        {
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
        }
    }


}

/* scaffolding [
    
    {
      "FileName": "DefaultQuery.cs",
      "Indexline": "Field<ProjectsSpecification>",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Field<ProjectsSpecification>("projects",resolve: context => Task.FromResult(new object()));"
      ]
    }
] scaffolding */
