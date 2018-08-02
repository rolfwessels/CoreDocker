using System;
using System.Reflection;
using CoreDocker.Api;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.Security;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi;
using log4net;
using Microsoft.AspNetCore.Hosting;

namespace CoreDocker.Sdk.Tests.Shared
{
    public class IntegrationTestsBase
    {
        
        public const string ClientId = "CoreDocker.Api";
        public const string AdminPassword = "admin!";
        public const string AdminUser = "admin@admin.com";
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected static readonly Lazy<string> _hostAddress;


        protected static Lazy<ConnectionFactory> _defaultRequestFactory;
        protected static Lazy<CoreDockerClient> _adminConnection;
        protected static Lazy<CoreDockerClient> _guestConnection;

        static IntegrationTestsBase()
        {
            RestShapHelper.Log = s => _log.Debug(s);
            _hostAddress = new Lazy<string>(StartHosting);
            _defaultRequestFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory(_hostAddress.Value));
            _adminConnection = new Lazy<CoreDockerClient>(() => CreateLoggedInRequest(AdminUser, AdminPassword));
            _guestConnection = new Lazy<CoreDockerClient>(() => CreateLoggedInRequest("Guest@Guest.com", "guest!"));
            
        }

        #region Private Methods

        private static string StartHosting()
        {
            var port = new Random().Next(9000, 9999);
            var address = string.Format("http://localhost:{0}", port);
            Environment.SetEnvironmentVariable("OpenId__HostUrl", address);
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureAppConfiguration(Program.SettingsFileReaderHelper)
                .UseStartup<Startup>()
                .UseUrls(address);
            host.Build().Start();
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            
            _log.Info(string.Format("Starting api on [{0}]", address));
            RestShapHelper.Log = m =>
            {
                _log.Debug(m);
            };
            return address;
        }


        private static CoreDockerClient CreateLoggedInRequest(string adminAdminCom, string adminPassword)
        {
            var coreDockerApi = _defaultRequestFactory.Value.GetConnection();
            coreDockerApi.Authenticate.Login(adminAdminCom, adminPassword).Wait();
            // add the authentication here
            return (CoreDockerClient) coreDockerApi;
        }

        #endregion
    }
}