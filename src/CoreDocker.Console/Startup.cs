using System;
using System.IO;
using System.Reflection;
using CoreDocker.Api.Security;
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
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            
            SecuritySetup.AddIndentityServer4(services);
            services.AddMvc();
            
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Config(loggerFactory, Configuration);
            if (env.IsDevelopment())
            {
//                app.UseDeveloperExceptionPage();
            }
            SecuritySetup.SetupMap(app);
          
            app.UseMvc();
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