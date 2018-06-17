using GraphQL.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class QueryResultSpecification : ObjectGraphType<IGraphQlQueryOptions>
    {
        public QueryResultSpecification()
        {
            Name = "QueryResult";
            Field<IntGraphType>(
                "count",
                Description = "total count",
                resolve: ResolveCount);
        }

        private object ResolveCount(ResolveFieldContext<IGraphQlQueryOptions> context)
        {
            return context.Source.Count(); 
        }
    }
}