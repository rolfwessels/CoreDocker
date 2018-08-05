using System.Collections.Generic;
using CoreDocker.Api.WebApi.Controllers;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl.DynamicQuery
{
    public class DataHolder<TController, TModel, TDal> : IGraphQlQueryOptions where TController : IQueryableControllerBase<TDal, TModel>
    {
        private readonly GraphQlQueryOptions<TController, TModel, TDal> _graphQlQueryOptions;
        private readonly ResolveFieldContext<object> _context;
        private readonly List<TModel> _buildQuery;

        public DataHolder(GraphQlQueryOptions<TController, TModel, TDal> graphQlQueryOptions, ResolveFieldContext<object> context)
        {
            _graphQlQueryOptions = graphQlQueryOptions;
            _context = context;
            _buildQuery = _graphQlQueryOptions.BuildQuery(context);
        }

        #region Implementation of IGraphQlQueryOptions

        public int Count()
        {
            return _graphQlQueryOptions.BuildCount(_context);
        }

        #endregion
    }
}