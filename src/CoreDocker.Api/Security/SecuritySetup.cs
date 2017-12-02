using Autofac;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.Security
{
    public class SecuritySetup
    {
        public static void AddIndentityServer4(IServiceCollection services)
        {
           // var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "damienbodserver.pfx"), "");

            
//
//            services.AddIdentity<ApplicationUser, IdentityRole>()
//                .AddEntityFrameworkStores<ApplicationDbContext>()
//                .AddDefaultTokenProviders()
//                .AddIdentityServer();

            var guestPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("scope", "dataEventRecords")
                .Build();


            //
            //            services.AddTransient<IEmailSender, AuthMessageSender>();
            //            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = OpenIdConfig.HostUrl;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = OpenIdConfig.ResourceName;
                });

            services.AddIdentityServer()
//                .AddSigningCredential(cert)
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(OpenIdConfig.GetIdentityResources())
                .AddInMemoryApiResources(OpenIdConfig.GetApiResources())
                .AddInMemoryClients(OpenIdConfig.GetClients())
                .AddTestUsers(OpenIdConfig.Users());




            services.AddAuthorization(options =>
            {
                options.AddPolicy("dataEventRecordsAdmin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "dataEventRecords.admin");
                });
                options.AddPolicy("admin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "admin");
                });
                options.AddPolicy("dataEventRecordsUser", policyUser =>
                {
                    policyUser.RequireClaim("role", "dataEventRecords.user");
                });
            });

        }

        public static void Add(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityWithAdditionalClaimsProfileService>().As<IProfileService>();
        }


        public static void SetupMap(IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseAuthentication();
        }
    }
}

