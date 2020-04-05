using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Base;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl.DynamicQuery
{
    public static class GraphQlQueryOptionsHelper
    {
        public static IObjectFieldDescriptor AddOptions<TDal, TOptions>(this IObjectFieldDescriptor description,
            GraphQlQueryOptions<TDal, TOptions> options) where TDal : IBaseDalModel
            where TOptions : PagedLookupOptionsBase, new()
        {
            return options.AddArguments(description);
        }
    }
}