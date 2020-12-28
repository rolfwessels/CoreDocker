using System;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Api.SignalR;
using CoreDocker.Api.SignalR.Hubs;
using CoreDocker.Api.Swagger;
using CoreDocker.Api.WebApi;
using CoreDocker.Api.WebApi.Filters;
using CoreDocker.Shared;
using CoreDocker.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>(SignalRHubUrls.ChatUrl);
            });

            app.AddGraphQl();
            app.UseSwagger();
            SimpleFileServer.Initialize(app);
        }

        public static string InformationalVersion()
        {
            return Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
        }
    }
}