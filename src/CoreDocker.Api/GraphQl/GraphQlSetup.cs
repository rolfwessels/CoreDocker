using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Authorization;
using GraphQL.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlSetup
    {
        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddGraphQLAuth();
            services.AddGraphQLHttp();
        }

        public static void AddGraphQl(this IApplicationBuilder app)
        {
            var settings = new GraphQLHttpOptions
            {
                BuildUserContext = ctx =>
                {
                    var userContext = new GraphQLUserContext
                    {
                        User = ctx.User
                    };

                    return Task.FromResult(userContext);
                }
            };

//            var rules = app.ApplicationServices.GetServices<IValidationRule>();
//            settings.ValidationRules = rules.ToArray();

            app.UseGraphQLHttp<ISchema>(settings);
            
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
        }


        public static void AddGraphQLAuth(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.TryAddSingleton(s =>
            {
                var authSettings = new AuthorizationSettings();

                authSettings.AddPolicy("AdminPolicy", _ => _.RequireClaim("role", "Admin"));

                return authSettings;
            });
        }


        public class GraphQLUserContext : IProvideClaimsPrincipal
        {
            public ClaimsPrincipal User { get; set; }
        }

        public class GraphQLSettings
        {
            public Func<HttpContext, Task<object>> BuildUserContext { get; set; }
            public object Root { get; set; }
            public List<IValidationRule> ValidationRules { get; } = new List<IValidationRule>();
        }
    }

    
}