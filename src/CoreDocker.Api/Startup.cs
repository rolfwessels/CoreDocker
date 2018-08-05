using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Api.SignalR;
using CoreDocker.Api.Swagger;
using CoreDocker.Api.WebApi;
using CoreDocker.Utilities;
using CoreDocker.Utilities.Helpers;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("logSettings.xml"));
            Configuration = configuration;
            Settings.Initialize(Configuration);
        }

        public IConfiguration Configuration { get; }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IocApi.Populate(services);
            services.AddGraphQl();
            services.AddCors();
            services.UseIndentityService(Configuration);
            services.AddBearerAuthentication();
            services.AddMvc(WebApiSetup.Setup);
            services.AddSwagger();
            services.AddSignalR();
            
            return new AutofacServiceProvider(IocApi.Instance.Container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {            
            app.UseCors(policy =>
            {
                policy.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(new OpenIdSettings(Configuration).GetOriginList());
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIndentityService();
            app.UseBearerAuthentication();
            app.UseSingalRSetup();
            app.UseMvc();
            app.AddGraphQl();
            app.UseSwagger();
            SimpleFileServer.Initialize(app);
        }

        public static string InformationalVersion()
        {
            try
            {
                return Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
            }
            catch (Exception e)
            {
                ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Error($"SwaggerSetup:InformationalVersion Failed to get the InformationalVersion  {e.Message}");
                return "1.0.0";
            }
        }
    }
}