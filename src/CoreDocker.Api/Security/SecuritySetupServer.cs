using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.Security
{
    public static class SecuritySetupServer
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void UseIdentityService(this IServiceCollection services, IConfiguration conf)
        {
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            var openIdSettings = new OpenIdSettings(conf);
            services.AddIdentityServer()
                .AddSigningCredential(Certificate())
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

        private static X509Certificate2 Certificate()
        {
            X509Certificate2 cert = null;
            using (var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    // Replace below with your cert's thumbprint
                    "EF255CFED88F0F825008BA9425AB0D469A73B01D",
                    false);
                // Get the first cert with the thumbprint
                if (certCollection.Count > 0)
                {
                    cert = certCollection[0];
                    _log.Info($"Successfully loaded cert from registry: {cert.Thumbprint}");
                }
            }

            // Fallback to local file for development
            if (cert == null)
            {
                var fileName = Path.Combine("./Certificates", "development.pfx");
                if (!File.Exists(fileName))
                    _log.Error(
                        $"SecuritySetupServer:Certificate Could not load file {fileName} to obtain the certificate.");

                cert = new X509Certificate2(fileName, "exportpassword");
                _log.Info($"Falling back to cert from file. Successfully loaded: {cert.Thumbprint}");
            }

            return cert;
        }

        #endregion
    }
}