using System;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Utilities.Helpers;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Execution.Options;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlSetup
    {
        public static void AddGraphQl(this IServiceCollection services)
        {
            // services.AddInMemorySubscriptionProvider();
            services.AddGraphQLServer()
                .AddErrorFilter<ErrorFilter>()
                .AddQueryType<DefaultQuery>()
                .AddMutationType<DefaultMutation>()
                .AddAuthorization();
            // .AddSubscriptionType<DefaultSubscription>();
        }

  


        public static void AddGraphQl(this IApplicationBuilder app)
        {
            var openIdSettings = IocApi.Instance.Resolve<OpenIdSettings>();
            var pathString = new Uri(openIdSettings.HostUrl.UriCombine("/graphql")).AbsolutePath;
            app.UseWebSockets();
            app.UseEndpoints(x => x.MapGraphQL());
            // app.UseGraphQLSubscriptions(new SubscriptionMiddlewareOptions() {Path = pathString});
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