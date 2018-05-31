using System;
using System.IO;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Api.SignalR;
using CoreDocker.Api.Swagger;
using CoreDocker.Api.WebApi;
using CoreDocker.Utilities;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("logSettings.xml"));
            Configuration = ReadAppSettings(env);
            Settings.Initialize(Configuration);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IocApi.Populate(services);
            services.AddCors();
            services.UseIndentityService();
            services.AddBearerAuthentication();
            services.AddMvc(WebApiSetup.Setup);
            services.AddSwagger();
            services.AddSignalR();
            return new AutofacServiceProvider(IocApi.Instance.Container);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(policy =>
            {
                policy.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200");
            });
            Config(loggerFactory, Configuration);

            SimpleFileServer.Initialize(app);
            app.UseIndentityService();
            app.UseBearerAuthentication();
            app.UseSingalRSetup();
            app.UseMvc();
            app.UseSwagger();
        }

        #region Private Methods

        private static void Config(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot)
        {
            loggerFactory.AddConsole(configurationRoot.GetSection("Logging"));
        }

        private IConfigurationRoot ReadAppSettings(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            builder.AddEnvironmentVariables();
            return builder.Build();
        }

        #endregion
    }
}