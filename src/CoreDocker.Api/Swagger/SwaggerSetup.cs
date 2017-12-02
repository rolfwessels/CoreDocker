using System.Linq;
using System.Reflection;
using CoreDocker.Utilities.FakeLogging;
using CoreDocker.Utilities.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Swagger.Model;

namespace CoreDocker.Api.Swagger
{
    public class SwaggerSetup
    {
        private static readonly ILog _log = LogManager.GetLogger<SwaggerSetup>();

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

        internal static void Setup(IServiceCollection services)
        {

            services.AddSwaggerGen(
                options => options.SingleApiVersion(new Info
                {
                    Title = $"CoreDocker API v{_informationalVersion}",
                    Version = GetVersion()
                }));  
            // todo: Rolf Add Auth response codes
        }

        internal static void AddUi(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUi(swaggerUrl: $"/swagger/{GetVersion()}/swagger.json");
        }

        #endregion
    }
}