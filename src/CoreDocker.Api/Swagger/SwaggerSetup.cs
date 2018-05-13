using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreDocker.Api.Security;
using log4net;
using CoreDocker.Utilities.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CoreDocker.Api.Swagger
{
    public static class SwaggerSetup
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string _informationalVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

        #region Private Methods

        private static string GetVersion()
        {
            _informationalVersion = _informationalVersion.Split('.').Take(2).StringJoin(".");
            var version = "v"+_informationalVersion;
            _log.Info("swagger version:"+ version);
            return version;
            ;
        }

        #endregion

        #region Instance

        public static void AddSwagger(this IServiceCollection services)
        {

            services.AddSwaggerGen(SetupAction);
            
            // todo: Rolf Add Auth response codes
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            SwaggerBuilderExtensions.UseSwagger(app);
//            app.UseSwaggerUi(swaggerUrl: $"/swagger/{GetVersion()}/swagger.json");
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{GetVersion()}/swagger.json", $"Main {GetVersion()}");
                c.OAuthClientId(OpenIdConfigBase.ClientName);
                c.OAuthClientSecret(OpenIdConfigBase.ClientSecret);
                c.OAuthClientSecret(OpenIdConfigBase.ClientSecret);
                c.OAuthClientSecret(OpenIdConfigBase.ClientSecret);
                c.OAuthRealm(OpenIdConfigBase.ApiResourceName);
                c.OAuthAppName("SwaggerAuth");
            });

        }

        #endregion

        private static void SetupAction(SwaggerGenOptions options)
        {
            options.SwaggerDoc(GetVersion(),new Info
            {
                Title = $"CoreDocker API",
                //                Version = GetVersion()
                Description = "Contains CoreDocker api descriptions.",
            });
            options.AddSecurityDefinition("oauth2", new OAuth2Scheme
            {
                Type = "oauth2",
                Flow = "password",
                TokenUrl = OpenIdConfigBase.HostUrl.UriCombine("connect/token"),
                AuthorizationUrl = OpenIdConfigBase.HostUrl,
                
                Scopes = new Dictionary<string, string>
                {
                    { OpenIdConfigBase.ScopeApi.UnderScoreAndCamelCaseToHumanReadable(), OpenIdConfigBase.ScopeApi }
                }
            });


        }
    }
}