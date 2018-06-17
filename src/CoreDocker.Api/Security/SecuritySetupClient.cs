using System;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Utilities.Helpers;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CoreDocker.Api.Security
{
    public static class SecuritySetupClient
    {
        public static void AddBearerAuthentication(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddAuthorization(AddFromActivities);
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = OpenIdConfigBase.HostUrl;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = OpenIdConfigBase.ApiResourceName;
                    options.ApiSecret = OpenIdConfigBase.ApiResourceSecret;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(5);
                });
        }
       


        private static void AddFromActivities(AuthorizationOptions options)
        {
            EnumHelper.ToArray<Activity>()
                .ForEach(activity =>
                {
                    options.AddPolicy(UserClaimProvider.ToPolicyName(activity), policyAdmin => {
                        policyAdmin.RequireClaim(JwtClaimTypes.Role, UserClaimProvider.ToPolicyName(activity));
                    });
                });
            
        }

        public static void UseBearerAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}

