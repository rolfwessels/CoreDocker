using System;
using System.Diagnostics;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Api.Swagger;
using CoreDocker.Api.WebApi.Filters;
using CoreDocker.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CoreDocker.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Settings.Initialize(Configuration);
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IocApi.Populate(services);
            services.AddGraphQl();
            services.AddCors();
            services.UseIdentityService(Configuration);
            services.AddBearerAuthentication();
            services.AddMvc(config => { config.Filters.Add(new CaptureExceptionFilter()); });
            services.AddSwagger();
            services.AddSignalR();

            return new AutofacServiceProvider(IocApi.Instance.Container);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
            log.Information("Starting server {InformationalVersion}", InformationalVersion());
            app.UseStaticFiles();
            app.UseRouting();
            var openIdSettings = new OpenIdSettings(Configuration);

            app.UseCors(policy =>
            {
                policy.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)) // Cache the OPTIONS calls.
                    .WithOrigins(openIdSettings.GetOriginList());
            });

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseIdentityService(openIdSettings);
            app.UseBearerAuthentication();
            app.UseAuthentication();
            app.UseAuthorization();
            app.AddGraphQl();
            app.UseEndpoints(e => e.MapControllers());
            app.UseSwagger();
            SimpleFileServer.Initialize(app);
            

        }

        public static string InformationalVersion()
        {
            // string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            // string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            string? productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            return $"{productVersion}";
        }
    }
}