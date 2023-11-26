using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Base;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl.DynamicQuery
{
    public class GraphQlQueryOptions<TDal, TOptions> where TDal : IBaseDalModel
        where TOptions : PagedLookupOptionsBase, new()
    {
        private const int DefaultLimit = 1000;
        private readonly List<ArgBase> _args;
        private readonly Func<TOptions, Task<PagedList<TDal>>> _lookup;


        public GraphQlQueryOptions(Func<TOptions, Task<PagedList<TDal>>> lookup)
        {
            _lookup = lookup;
            _args = new List<ArgBase>
            {
                new Arg<IntType>("first", "Set the limit to return.",
                    (options, context) => options.First = context.ArgumentValue<int?>("first") ?? DefaultLimit),
                new Arg<BooleanType>("includeCount", "Select to return the count of the items.",
                    (options, context) => options.IncludeCount = context.ArgumentValue<bool>("includeCount"))
            };
        }


        public Task<PagedList<TDal>> Paged(IResolverContext context, TOptions? options = null)
        {
            options = options ?? new TOptions();

            foreach (var arg in _args)
            {
                arg.Apply(options, context);
            }

            return _lookup(options);
        }

        public IEnumerable<TDal> Query(IResolverContext resolverContext)
        {
            var options1 = new TOptions();
            return _lookup(options1).Result.Items;
        }

        public IObjectFieldDescriptor AddArguments(IObjectFieldDescriptor description)
        {
            foreach (var argBase in _args)
            {
                argBase.Apply(description);
            }

            return description;
        }

        public GraphQlQueryOptions<TDal, TOptions> AddArguments<TIn>(string name,
            string description,
            Action<TOptions, IResolverContext> applyArgument) where TIn : IInputType
        {
            _args.Add(new Arg<TIn>(name, description, applyArgument));
            return this;
        }

        public class Arg<TIn> : ArgBase where TIn : IInputType
        {
            public Arg(string name, string description, Action<TOptions, IResolverContext> applyArgument)
                : base(name, description, applyArgument)
            {
            }

            public override void Apply(IObjectFieldDescriptor description)
            {
                description.Argument(Name, x => x.Type<TIn>().Description(Description));
            }
        }

        public abstract class ArgBase
        {
            protected ArgBase(string name, string description, Action<TOptions, IResolverContext> applyArgument)
            {
                Name = name;
                Description = description;
                ApplyArgument = applyArgument;
            }

            public string Name { get; }
            public string Description { get; }
            public Action<TOptions, IResolverContext> ApplyArgument { get; }
            public abstract void Apply(IObjectFieldDescriptor description);

            public void Apply(TOptions description, IResolverContext context)
            {
                ApplyArgument(description, context);
            }
        }
    }
}