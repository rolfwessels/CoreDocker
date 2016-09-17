using System;
using System.Diagnostics;
using CoreDocker.Api;
using Microsoft.AspNetCore.Hosting;
using log4net;

namespace CoreDocker.Sdk.Tests.Shared
{
    public class IntegrationTestsBase
    {
        public const string ClientId = "CoreDocker.Api";
        public const string AdminPassword = "admin!";
        public const string AdminUser = "admin";
        private static ILog _log = LogManager.GetLogger<IntegrationTestsBase>();
        private static readonly Lazy<string> _hostAddress;


        protected static Lazy<ConnectionFactory> _defaultRequestFactory;
        protected static Lazy<ConnectionFactory> _adminRequestFactory;

        static IntegrationTestsBase()
        {
            _hostAddress = new Lazy<string>(StartHosting);
            _defaultRequestFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory(_hostAddress.Value));
            _adminRequestFactory = new Lazy<ConnectionFactory>(CreateAdminRequest);
        }

        #region Private Methods

        private static string StartHosting()
        {
            var port = new Random().Next(9000, 9999);
            var address = string.Format("http://localhost:{0}", port);
//            var websitePath = TestHelper.GetSourceBasePath();

            var host = new WebHostBuilder()
                .UseKestrel()
//                .UseContentRoot(websitePath)
                .UseStartup<Startup>()
                .UseUrls(address);
            host.Build().Start();
            _log = LogManager.GetLogger<IntegrationTestsBase>();
            _log.Info(string.Format("Starting api on [{0}]", address));
            FlurlHelper.Log = m =>
            {
                _log.Info(m);
                Debug.WriteLine("value [{0}]", m);
            };
            FlurlHelper.LogError = m =>
            {
                _log.Error(m);
                Debug.WriteLine("value [{0}]", m);
            };
            return address;
        }


        private static ConnectionFactory CreateAdminRequest()
        {
            var restConnectionFactory = new ConnectionFactory(_hostAddress.Value);
            // add the authentication here
            return restConnectionFactory;
        }

        #endregion
    }
}