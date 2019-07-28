using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.Security
{
    public static class SecuritySetupServer
    {

        public static void UseIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            var openIdSettings = new OpenIdSettings(configuration);
            Log.Debug($"SecuritySetupServer:UseIdentityService Setting the host url {openIdSettings.HostUrl}");
            services.AddIdentityServer(x => { x.PublicOrigin = openIdSettings.HostUrl; })
                .AddSigningCredential(Certificate(openIdSettings.CertPfx, openIdSettings.CertPassword,
                    openIdSettings.CertStoreThumbprint))
                .AddInMemoryIdentityResources(OpenIdConfig.GetIdentityResources())
                .AddInMemoryApiResources(OpenIdConfig.GetApiResources(openIdSettings))
                .AddInMemoryClients(OpenIdConfig.GetClients(openIdSettings))
                // options => options.MigrationsAssembly(migrationsAssembly))) 
                .Services.AddTransient<IResourceOwnerPasswordValidator, UserClaimProvider>();
        }

        public static void UseIdentityService(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }

        #region Private Methods

        private static X509Certificate2 Certificate(string certFile, string password, string certStoreThumbprint)
        {
            try
            {
                X509Certificate2 cert = null;
                if (!string.IsNullOrEmpty(certStoreThumbprint))
                    using (var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                    {
                        certStore.Open(OpenFlags.ReadOnly);
                        var certCollection = certStore.Certificates.Find(
                            X509FindType.FindByThumbprint,
                            certStoreThumbprint,
                            false);
                        // Get the first cert with the thumbprint
                        if (certCollection.Count > 0)
                        {
                            cert = certCollection[0];
                            Log.Information($"Successfully loaded cert from registry: {cert.Thumbprint}");
                        }
                    }

                // Fallback to local file for development
                if (cert == null)
                {
                    var fileName = Path.Combine("./Certificates", certFile);
                    if (!File.Exists(fileName))
                    {
                        Log.Error(
                            $"SecuritySetupServer:Certificate Could not load file {Path.GetFullPath(fileName)} to obtain the certificate.");
                    }
                    else
                    {
                        cert = new X509Certificate2(fileName, password);
                        Log.Information($"Falling back to cert from file. Successfully loaded: {cert.Thumbprint}");
                    }
                }

                return cert;
            }
            catch (Exception e)
            {
                Log.Error($"SecuritySetupServer:Certificate {e.Message}");
                throw;
            }
        }

        #endregion
    }
}
