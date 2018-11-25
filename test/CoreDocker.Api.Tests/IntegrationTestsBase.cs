using System;
using System.Reflection;
using CoreDocker.Sdk;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi;
using log4net;
using Microsoft.AspNetCore.Hosting;

namespace CoreDocker.Api.Tests
{
    public class IntegrationTestsBase
    {
        public const string ClientId = "CoreDocker.Api";
        public const string AdminPassword = "admin!";
        public const string AdminUser = "admin@admin.com";
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected static readonly Lazy<string> HostAddress;


        protected static Lazy<ConnectionFactory> _defaultRequestFactory;
        protected static Lazy<CoreDockerClient> _adminConnection;
        protected static Lazy<CoreDockerClient> _guestConnection;

        static IntegrationTestsBase()
        {
            RestShapHelper.Log = s => _log.Debug(s);
            HostAddress = new Lazy<string>(StartHosting);
            _defaultRequestFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory(HostAddress.Value));
            _adminConnection = new Lazy<CoreDockerClient>(() => CreateLoggedInRequest(AdminUser, AdminPassword));
            _guestConnection = new Lazy<CoreDockerClient>(() => CreateLoggedInRequest("Guest@Guest.com", "guest!"));
        }

        #region Private Methods

        private static string StartHosting()
        {
            var port = new Random().Next(9000, 9999);
            var address = $"http://localhost:{port}";
            Environment.SetEnvironmentVariable("OpenId__HostUrl", address);
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureAppConfiguration(Program.SettingsFileReaderHelper)
                .UseStartup<Startup>()
                .UseUrls(address);
            host.Build().Start();
            
            _log.Info($"Starting api on [{address}]");
            RestShapHelper.Log = m => { _log.Debug(m); };
            return address;
        }


        private static CoreDockerClient CreateLoggedInRequest(string adminAdminCom, string adminPassword)
        {
            var coreDockerApi = _defaultRequestFactory.Value.GetConnection();
            coreDockerApi.Authenticate.Login(adminAdminCom, adminPassword).Wait();
            return (CoreDockerClient) coreDockerApi;
        }

        protected CoreDockerClient NewClientNotAuthorized()
        {
            return (CoreDockerClient)_defaultRequestFactory.Value.GetConnection();
        }

        #endregion
    }
}