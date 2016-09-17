using System;
using System.Globalization;
using System.Linq;
using System.Net;
using CoreDocker.Api.WebApi.Controllers;
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
            return Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;
        }

        #endregion

        #region Instance

        internal static void Setup(IServiceCollection services)
        {
            services.AddSwaggerGen();
            // todo: Rolf Add version information  
            // todo: Rolf Add Auth response codes
        }

        internal static void AddUi(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUi("swagger/ui");
        }

        #endregion
    }
}