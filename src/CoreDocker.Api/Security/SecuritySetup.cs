using Autofac;
using CoreDocker.Utilities.Helpers;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.Security
{
    public class SecuritySetup
    {
        public static void AddIndentityServer4(IServiceCollection services)
        {
            services.AddCors();
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

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = OpenIdConfig.HostUrl.Dump("csad");
                    options.RequireHttpsMetadata = false;
                    options.ApiName = OpenIdConfig.ResourceName;
                    options.ApiSecret = "secret";
                });

          
        }

        public static void Add(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityWithAdditionalClaimsProfileService>().As<IProfileService>();
        }


        public static void SetupMap(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithExposedHeaders("WWW-Authenticate");
            });
        }
    }
}

