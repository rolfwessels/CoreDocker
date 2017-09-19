using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Swagger.Model;

namespace CoreDocker.Api.Swagger
{
    public class SwaggerSetup
    {
        #region Private Methods

        private static string GetVersion()
        {
            return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            ;
        }

        #endregion

        #region Instance

        internal static void Setup(IServiceCollection services)
        {
            services.AddSwaggerGen(
                options => options.SingleApiVersion(new Info { Contact = new Contact(){Name = "Rolf"}}));
            // todo: Rolf Add version information  
            // todo: Rolf Add Auth response codes
        }

        internal static void AddUi(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUi();
        }

        #endregion
    }
}