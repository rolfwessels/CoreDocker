using System;
using System.Collections.Generic;
using System.Linq;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreDocker.Api.Swagger
{
    public static class SwaggerSetup
    {
        private static string _informationalVersion = EnvHelper.InformationalVersion();

        private static string GetVersion()
        {
            _informationalVersion = _informationalVersion.Split('.').Take(2).StringJoin(".");
            var version = "v" + _informationalVersion;
            return version;
        }

        private static void SetupAction(SwaggerGenOptions options, string authorizationUrl)
        {
            options.SwaggerDoc(GetVersion(), new OpenApiInfo
            {
                Title = "CoreDocker API",
                Description = "Contains CoreDocker api descriptions."
            });
            var scopeApi = OpenIdConfig.Scope;


            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                In = ParameterLocation.Header,


                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorizationUrl.UriCombine("connect/token"),
                            UriKind.Absolute),
                        TokenUrl = new Uri(authorizationUrl.UriCombine("connect/token"),
                            UriKind.Absolute),
                        Scopes = new Dictionary<string, string>
                        {
                            { scopeApi.UnderScoreAndCamelCaseToHumanReadable(), scopeApi }
                        }
                    }
                }
            });
        }

        public static void AddSwagger(this IServiceCollection services, OpenIdSettings openIdSettings)
        {
            services.AddSwaggerGen(options => SetupAction(options, openIdSettings.HostUrl));
            // todo: Rolf Add Auth response codes
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            var openIdSettings = app.ApplicationServices.GetRequiredService<OpenIdSettings>();
            SwaggerBuilderExtensions.UseSwagger(app);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(openIdSettings.HostUrl.UriCombine($"/swagger/{GetVersion()}/swagger.json"),
                    $"Main {GetVersion()}");
                c.OAuthClientId(openIdSettings.ClientName);
                var clientSecret = openIdSettings.ClientSecret;
                c.OAuthClientSecret(clientSecret);
                c.OAuthClientSecret(clientSecret);
                c.OAuthClientSecret(clientSecret);
                c.OAuthRealm(openIdSettings.ApiResourceName);
                c.OAuthAppName("SwaggerAuth");
            });
        }
    }
}