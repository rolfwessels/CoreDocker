using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Utilities.Helpers;
using GraphQL;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Authorization;
using GraphQL.Language.AST;
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
    

        /// <summary>
        ///     Keep dates in the original format.
        /// </summary>
        public class OriginalDateGraphType : DateGraphType
    {
            public OriginalDateGraphType()
            {
                Name = "Date";
                Description =
                    "The `Date` scalar type represents a timestamp provided in UTC. `Date` expects timestamps " +
                    "to be formatted in accordance with the [ISO-8601](https://en.wikipedia.org/wiki/ISO_8601) standard.";
                
            }

            public override object Serialize(object value)
            {
                return ParseValue(value);
            }

            public override object ParseValue(object value)
            {
                if (value is DateTime time)
                {
                    return time.ToUniversalTime();
                }

                var inputValue = value?.ToString().Trim('"');

                if (DateTime.TryParse(
                    inputValue,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.NoCurrentDateDefault,
                    out var outputValue))
                {
                    return outputValue.ToUniversalTime();
                }

                return null;
            }

            public override object ParseLiteral(IValue value)
            {
                var timeValue = value as DateTimeValue;
                if (timeValue != null)
                {
                    return ParseValue(timeValue.Value);
                }

                var stringValue = value as StringValue;
                if (stringValue != null)
                {
                    return ParseValue(stringValue.Value);
                }

                return null;
            }
        
    }

    
}