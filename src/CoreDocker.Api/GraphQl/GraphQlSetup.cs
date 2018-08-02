using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Utilities.Helpers;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
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
            var rules = app.ApplicationServices.GetServices<IValidationRule>();
            rules.ForEach(x => settings.ValidationRules.Add(x));
            
            app.UseGraphQLHttp<ISchema>(settings);
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
        }


       

        #region Nested type: GraphQLSettings

        public class GraphQLSettings
        {
            public Func<HttpContext, Task<object>> BuildUserContext { get; set; }
            public object Root { get; set; }
            public List<IValidationRule> ValidationRules { get; } = new List<IValidationRule>();
        }

        #endregion

        #region Nested type: GraphQLUserContext

        public class GraphQLUserContext : IProvideClaimsPrincipal
        {
            public ClaimsPrincipal User { get; set; }
        }

        #endregion
    }
}