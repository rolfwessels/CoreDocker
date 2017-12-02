using System;
using Autofac;
using CoreDocker.Core;
using CoreDocker.Utilities.Helpers;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.Security
{
    public static class SecuritySetupClient
    {
        public static void AddBearerAuthentication(this IServiceCollection services)
        {
            
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
                    options.Authority = OpenIdConfigBase.HostUrl.Dump("---------------->");
                    options.RequireHttpsMetadata = false;
                    options.ApiName = OpenIdConfigBase.ResourceName;
                    options.ApiSecret = "secret";
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(5);
                });
 
        }

        public static void Add(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityWithAdditionalClaimsProfileService>().As<IProfileService>();
        }


        public static void UseBearerAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            
        }
    }
}

