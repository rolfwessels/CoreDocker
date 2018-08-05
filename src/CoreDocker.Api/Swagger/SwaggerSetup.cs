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
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string _informationalVersion = Startup.InformationalVersion();

      

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
                c.SwaggerEndpoint(openIdSettings.HostUrl.UriCombine($"/swagger/{GetVersion()}/swagger.json") , $"Main {GetVersion()}");
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
        #region Private Methods

        private static string GetVersion()
        {
            _informationalVersion = _informationalVersion.Split('.').Take(2).StringJoin(".");
            var version = "v" + _informationalVersion;
            return version;
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
    }
}