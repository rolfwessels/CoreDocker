using System;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Utilities.Helpers;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlSetup
    {
        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddInMemorySubscriptions();
            services.AddGraphQLServer()
                .AddQueryType<DefaultQuery>()
                .AddMutationType<DefaultMutation>()
                .AddSubscriptionType<DefaultSubscription>()
                .AddAuthorization();
        }

        public static void AddGraphQl(this IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseEndpoints(x => x.MapGraphQL());
            app.UsePlayground("/graphql", "/ui/playground");
        }
    }
}