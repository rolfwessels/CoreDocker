using CoreDocker.Core;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.Security
{
    public static class SecuritySetupServer
    {
        public static void UseIndentityService(this IServiceCollection services)
        {
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            services.AddIdentityServer()
//                .AddSigningCredential(cert)
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(OpenIdConfig.GetIdentityResources())
                .AddInMemoryApiResources(OpenIdConfig.GetApiResources())
                .AddInMemoryClients(OpenIdConfig.GetClients())
                
                
                // options => options.MigrationsAssembly(migrationsAssembly))) 
                .Services.AddTransient<IResourceOwnerPasswordValidator, UserClaimProvider>();
        }
        
        public static void UseIndentityService(this IApplicationBuilder app)
        {
            app.UseIdentityServer();

        }
    }
}