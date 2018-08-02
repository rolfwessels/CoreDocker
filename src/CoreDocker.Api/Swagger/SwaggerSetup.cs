using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Utilities.Helpers;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreDocker.Api.Swagger
{
    public static class SwaggerSetup
    {
        private static string _informationalVersion = Assembly.GetEntryAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

        #region Private Methods

        private static string GetVersion()
        {
            _informationalVersion = _informationalVersion.Split('.').Take(2).StringJoin(".");
            var version = "v" + _informationalVersion;
            return version;
            ;
        }

        private static void SetupAction(SwaggerGenOptions options, string authorizationUrl)
        {
            options.SwaggerDoc(GetVersion(), new Info
            {
                Title = "CoreDocker API",
                Description = "Contains CoreDocker api descriptions."
            });
            options.DescribeAllEnumsAsStrings();
            var scopeApi = IocApi.Instance.Resolve<OpenIdSettings>().ScopeApi;
            options.AddSecurityDefinition("oauth2", new OAuth2Scheme
            {
                Type = "oauth2",
                Flow = "password",
                TokenUrl = authorizationUrl.UriCombine("connect/token"),
                AuthorizationUrl = authorizationUrl,

                Scopes = new Dictionary<string, string>
                {
                    {scopeApi.UnderScoreAndCamelCaseToHumanReadable(), scopeApi}
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
            SwaggerBuilderExtensions.UseSwagger(app);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{GetVersion()}/swagger.json", $"Main {GetVersion()}");
                c.OAuthClientId(IocApi.Instance.Resolve<OpenIdSettings>().ClientName);
                var clientSecret = IocApi.Instance.Resolve<OpenIdSettings>().ClientSecret;
                c.OAuthClientSecret(clientSecret);
                c.OAuthClientSecret(clientSecret);
                c.OAuthClientSecret(clientSecret);
                c.OAuthRealm(IocApi.Instance.Resolve<OpenIdSettings>().ApiResourceName);
                c.OAuthAppName("SwaggerAuth");
            });
        }

        #endregion
    }
}