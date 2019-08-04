using System;
using CoreDocker.Api.Components.Users;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlSetup
    {

        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddGraphQL(SchemaFactory,ConfigureBuilder);
        }

        private static ISchema SchemaFactory(IServiceProvider sp)
        {
            return SchemaBuilder.New()
                .AddQueryType<DefaultQuery>()
                .AddMutationType<DefaultMutation>()
//                .AddSubscriptionType<DefaultSubscription>()
                .AddServices(sp)
                .Create();
        }

        private static IQueryExecutionBuilder ConfigureBuilder(IQueryExecutionBuilder builder)
        {
            var queryExecutionOptionsAccessor = new QueryExecutionOptions
            {
                TracingPreference = TracingPreference.Always,
                IncludeExceptionDetails = true
            };
            return builder.UseDefaultPipeline(queryExecutionOptionsAccessor)
                .AddErrorFilter<ErrorFilter>();
        }

        public static void AddGraphQl(this IApplicationBuilder app)
        {
//            var openIdSettings = IocApi.Instance.Resolve<OpenIdSettings>();
//            var uriCombine = new Uri(openIdSettings.HostUrl.UriCombine("/graphql"));
            var pathString = "/graphql";
            app.UseGraphQL(pathString) ;
            app.UsePlayground(pathString, "/ui/playground"); 
        }
    }

}
