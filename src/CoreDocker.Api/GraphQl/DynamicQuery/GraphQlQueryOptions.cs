using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl.DynamicQuery
{
    public class GraphQlQueryOptions<TDal, TOptions> where TDal : IBaseDalModel where TOptions:PagedLookupOptionsBase, new()
    {
        private readonly Func<TOptions, Task<PagedList<TDal>>> _lookup;
        private List<Arg> _args;


        public GraphQlQueryOptions(Func<TOptions, Task<PagedList<TDal>>> lookup)
        {
            _lookup = lookup;
            _args = new List<Arg>();
            AddArgument(new QueryArgument<IntGraphType>
            {
                Name = "first",
                Description = "Set the limit to return."
            }, (options, context) => options.First = context.GetArgument<int>("First"));
        }

        public class Arg
        {
            public QueryArgument Argument { get; set; }
            public Action<TOptions, ResolveFieldContext<object>> Apply { get; set; }
        }

        public Task<PagedList<TDal>> Paged(ResolveFieldContext<object> context, TOptions options = null)
        {
            options = options ?? new TOptions();
            foreach (var arg in _args)
            {
                if (context.HasArgument(arg.Argument.Name))
                    arg.Apply(options, context);
            }

            return _lookup(options);
        }

        public async Task<List<TDal>> Query(ResolveFieldContext<object> context, TOptions options = null)
        {
            var options1 = options ?? new TOptions();
            options1.IncludeCount = false;
            var paged = await Paged(context, options1);
            return paged.Items;
        }

       

        public GraphQlQueryOptions<TDal, TOptions> AddArgument(QueryArgument queryArguments, Action<TOptions,ResolveFieldContext<object>> apply)
        {
            _args.Add(new Arg() {Argument = queryArguments , Apply = apply });
            return this;
        }

        public QueryArguments GetArguments()
        {
            return new QueryArguments(_args.Select(x=>x.Argument));
        }

        
    }
}