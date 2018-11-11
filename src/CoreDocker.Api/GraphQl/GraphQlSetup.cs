using System;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Utilities.Helpers;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlSetup
    {
        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddGraphQL(_ =>
                {
                    _.EnableMetrics = false;
                    _.ExposeExceptions = false;
                })
                .AddUserContextBuilder(GraphQlUserContext.BuildFromHttpContext)
                .AddWebSockets()
                .AddDataLoader();
        }

        public static void AddGraphQl(this IApplicationBuilder app)
        {
            var openIdSettings = IocApi.Instance.Resolve<OpenIdSettings>();
            var uriCombine = new Uri(openIdSettings.HostUrl.UriCombine("/graphql"));

            app.UseWebSockets();
            app.UseGraphQLWebSockets<ISchema>();
            app.UseGraphQL<ISchema>();

            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions {GraphQLEndPoint = uriCombine.PathAndQuery});
        }
    }
}