using GraphQL.Types;

namespace CoreDocker.Api.GraphQl.DynamicQuery
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