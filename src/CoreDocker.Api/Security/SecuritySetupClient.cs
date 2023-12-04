using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Models.Auth;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;

namespace CoreDocker.Api.Security
{
    public static class SecuritySetupClient
    {
        public static void AddAuthenticationClient(this IServiceCollection services, OpenIdSettings idSettings)
        {
            services.AddDistributedMemoryCache();
            services.AddAuthorization(AddFromActivities);
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = idSettings.HostUrl;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = idSettings.ApiResourceName;
                    options.ApiSecret = idSettings.ApiResourceSecret;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(5);
                });
        }

        public static void UseBearerAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
        }

        private static void AddFromActivities(AuthorizationOptions options)
        {
            EnumHelper.ToArray<Activity>()
                .ForEach(activity =>
                {
                    options.AddPolicy(UserClaimProvider.ToPolicyName(activity),
                        policyAdmin =>
                        {
                            policyAdmin.RequireClaim(JwtClaimTypes.Role, UserClaimProvider.ToPolicyName(activity));
                        });
                });
        }
    }
}