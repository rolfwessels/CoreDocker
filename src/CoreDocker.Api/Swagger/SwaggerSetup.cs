﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using Bumbershoot.Utilities.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreDocker.Api.Swagger
{
    public static class SwaggerSetup
    {
        private static string _informationalVersion = Startup.InformationalVersion();

        #region Private Methods

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
            var scopeApi = IocApi.Instance.Resolve<OpenIdSettings>().ScopeApi;


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
                            {scopeApi.UnderScoreAndCamelCaseToHumanReadable(), scopeApi}
                        }
                    }
                }
            });
        }

        #endregion

        #region Instance

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options => SetupAction(options, IocApi.Instance.Resolve<OpenIdSettings>().HostUrl));
            // todo: Rolf Add Auth response codes
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            var openIdSettings = IocApi.Instance.Resolve<OpenIdSettings>();
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

        #endregion
    }
}