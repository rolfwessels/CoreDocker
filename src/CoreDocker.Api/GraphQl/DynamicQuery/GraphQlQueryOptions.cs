using System.Collections.Generic;
using CoreDocker.Api.WebApi.Controllers;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl.DynamicQuery
{
    public class GraphQlQueryOptions<TController, TModel, TDal>
        where TController : IQueryableControllerBase<TDal, TModel>
    {
        private readonly TController _projects;

        public GraphQlQueryOptions(TController projects)
        {
            _projects = projects;
        }

        public QueryArguments GetArguments()
        {
            return new QueryArguments(
                new QueryArgument<IntGraphType>
                {
                    Name = "first",
                    Description = "id of the project"
                }
            );
        }

        public IGraphQlQueryOptions Query(ResolveFieldContext<object> context)
        {
            return new DataHolder<TController, TModel, TDal>(this, context);
        }

        public List<TModel> BuildQuery(ResolveFieldContext<object> context)
        {
            return _projects.Query(dals => dals);
        }

        public int BuildCount(ResolveFieldContext<object> context)
        {
            return _projects.Count(dals => dals);
        }
    }
}