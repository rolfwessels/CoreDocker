using System;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.Security;
using CoreDocker.Utilities.Helpers;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlSetup
    {

        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddInMemorySubscriptionProvider();
            services.AddGraphQL(SchemaFactory, ConfigureBuilder);
        }

        private static ISchema SchemaFactory(IServiceProvider sp)
        {
            return SchemaBuilder.New()
                .AddQueryType<DefaultQuery>()
                .AddMutationType<DefaultMutation>()
                .AddSubscriptionType<DefaultSubscription>()
                .AddAuthorizeDirectiveType()
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
            var openIdSettings = IocApi.Instance.Resolve<OpenIdSettings>();
            var pathString = new Uri(openIdSettings.HostUrl.UriCombine("/graphql")).AbsolutePath;
            app.UseGraphQL(pathString);

//            app.UseGraphQLSubscriptions(new SubscriptionMiddlewareOptions() {Path = pathString.UriCombine("ws")});
            app.UseGraphQLSubscriptions(new SubscriptionMiddlewareOptions() {Path = pathString});
            app.UsePlayground(new PlaygroundOptions()
                {
                    QueryPath = pathString,
                    Path = "/ui/playground",
                    EnableSubscription = true,
                    SubscriptionPath = pathString
                }
            );
        }
    }

}
