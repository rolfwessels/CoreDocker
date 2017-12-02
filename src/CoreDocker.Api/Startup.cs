using System;
using System.IO;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Api.Swagger;
using CoreDocker.Api.WebApi;
using CoreDocker.Console;
using CoreDocker.Core;
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
        public Startup(IHostingEnvironment env)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("logSettings.xml"));
            Configuration = ReadAppSettings(env);
            Settings.Initialize(Configuration);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IocApi.Populate(services);
            services.AddCors();
            services.UseIndentityService();
            services.AddBearerAuthentication();
            services.AddMvc(WebApiSetup.Setup);
            SwaggerSetup.Setup(services);
            return new AutofacServiceProvider(IocApi.Instance.Container);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithExposedHeaders("WWW-Authenticate");
            });
            Config(loggerFactory, Configuration);
            if (env.IsDevelopment())
            {
//                app.UseDeveloperExceptionPage();
            }

            app.UseIndentityService();
            app.UseBearerAuthentication();
            app.UseMvc();


            //            app.UseBearerAuthentication();
            //            app.UseMvc();
            SwaggerSetup.AddUi(app);
            SimpleFileServer.Initialize(app);
            
        }

        #region Private Methods

        private static void Config(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
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