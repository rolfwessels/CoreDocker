using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Api.SignalR;
using CoreDocker.Api.Swagger;
using CoreDocker.Api.WebApi;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Dal.Models;
using CoreDocker.Utilities;
using CoreDocker.Utilities.Helpers;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Types;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            RegisterGraphQl(services);


            services.AddCors();
            services.UseIndentityService();
            services.AddBearerAuthentication();
            services.AddMvc(WebApiSetup.Setup);
            services.AddSwagger();
            services.AddSignalR();
         
            
            return new AutofacServiceProvider(IocApi.Instance.Container);
        }

        private static void RegisterGraphQl(IServiceCollection services)
        {
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            
            services.AddSingleton<DefaultQuery>();
            services.AddSingleton<DefaultMutation>();

            services.AddSingleton<ProjectSpecification>();
            services.AddSingleton<ProjectsSpecification>();
            services.AddSingleton<ProjectCreateUpdateSpecification>();
            services.AddSingleton<ProjectsMutationSpecification>();

            services.AddSingleton<UserInterface>();
            services.AddSingleton<ISchema, DefaultSchema>();
            

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddGraphQLHttp();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {            
            app.UseCors(policy =>
            {
                policy.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200");
            });
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
            }

            app.UseIndentityService();
            app.UseBearerAuthentication();
            app.UseSingalRSetup();
            app.UseMvc();
            app.UseGraphQLHttp<ISchema>(new GraphQLHttpOptions());
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
            app.UseSwagger();
            SimpleFileServer.Initialize(app);
        }

        public class GraphQLUserContext
        {
            public ClaimsPrincipal User { get; set; }
        }
    }


}