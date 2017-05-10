﻿using System;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using CoreDocker.Api.Swagger;

namespace CoreDocker.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
//                builder.AddApplicationInsightsSettings(true);
            }


            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IocApi.Populate(services);
            // Add framework services.
//            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc(options => WebApiSetup.Setup(options));
            SwaggerSetup.Setup(services);
            
            SimpleFileServer.Initialize(services);
            return new AutofacServiceProvider(IocApi.Instance.Container);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.RollingFile(System.IO.Path.Combine(@"C:\temp\logs", "CoreDocker.Api.log"))
            .CreateLogger();

            log4net.LogManager.SetLogger(loggerFactory);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

//            app.UseApplicationInsightsRequestTelemetry();
//            app.UseApplicationInsightsExceptionTelemetry();
            app.UseMvc();
            SwaggerSetup.AddUi(app);
        }
    }
}