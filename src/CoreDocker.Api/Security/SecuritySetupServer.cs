using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core;
using IdentityServer4.Models;
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

    public class PersistedGrantStore : IPersistedGrantStore
    {
        #region Implementation of IPersistedGrantStore

        public Task StoreAsync(PersistedGrant grant)
        {
            throw new System.NotImplementedException();
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAsync(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}