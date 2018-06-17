using GraphQL;
using GraphQL.Server.Transports.AspNetCore;
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
            services.AddGraphQLHttp();
        }

        public static void AddGraphQl(this IApplicationBuilder app)
        {
            app.UseGraphQLHttp<ISchema>(new GraphQLHttpOptions());
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
        }
    }
}