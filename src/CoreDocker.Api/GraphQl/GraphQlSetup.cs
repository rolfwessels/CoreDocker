using System;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Utilities.Helpers;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlSetup
    {
        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddGraphQL(sp => SchemaBuilder.New()
                .AddQueryType<DefaultQuery>()
                .AddMutationType<DefaultMutation>()
                .AddSubscriptionType<DefaultSubscription>()
                .AddServices(sp)
                .Create(), new QueryExecutionOptions {TracingPreference = TracingPreference.Always});
        }

        public static void AddGraphQl(this IApplicationBuilder app)
        {
            var openIdSettings = IocApi.Instance.Resolve<OpenIdSettings>();
            var uriCombine = new Uri(openIdSettings.HostUrl.UriCombine("/graphql"));
            app.UseGraphQL(uriCombine.PathAndQuery) ;
            app.UsePlayground(); 
        }
    }
}
