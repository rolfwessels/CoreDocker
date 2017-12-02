using System;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Api.Swagger;
using CoreDocker.Api.WebApi;
using CoreDocker.Api.WebApi.Controllers;
using CoreDocker.Utilities;
using CoreDocker.Utilities.FakeLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoreDocker.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = ReadAppSettings(env);
            Settings.Initialize(Configuration);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IocApi.Populate(services);
            SecuritySetup.AddIndentityServer4(services);
            services.AddMvc(WebApiSetup.Setup);

            SwaggerSetup.Setup(services);

            return new AutofacServiceProvider(IocApi.Instance.Container);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Config(loggerFactory, Configuration);
            if (env.IsDevelopment())
            {
//                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseMvc();
            SwaggerSetup.AddUi(app);
            SimpleFileServer.Initialize(app);
        }

        #region Private Methods

        private static void Config(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurationRoot)
                .CreateLogger();

            LogManager.SetLogger(loggerFactory);

            loggerFactory.AddConsole(configurationRoot.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();
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