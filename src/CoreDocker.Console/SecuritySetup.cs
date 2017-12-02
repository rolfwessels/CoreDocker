using Autofac;
using CoreDocker.Console;
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

            services.AddIdentityServer()
//                .AddSigningCredential(cert)
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(OpenIdConfig.GetIdentityResources())
                .AddInMemoryApiResources(OpenIdConfig.GetApiResources())
                .AddInMemoryClients(OpenIdConfig.GetClients())
                .AddTestUsers(OpenIdConfig.Users());

            
        }
        

        public static void SetupMap(IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseAuthentication();
        }
    }
}

