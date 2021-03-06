﻿using System;
using CoreDocker.Dal.Tests;
using CoreDocker.Sdk;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Api.Tests
{
    public class IntegrationTestsBase
    {
        public const string ClientId = "CoreDocker.Api";
        public const string AdminPassword = "admin!";
        public const string AdminUser = "admin@admin.com";
        protected static readonly Lazy<string> HostAddress;

        protected static Lazy<ConnectionFactory> _defaultRequestFactory;
        protected static Lazy<CoreDockerClient> _adminConnection;
        protected static Lazy<CoreDockerClient> _guestConnection;

        static IntegrationTestsBase()
        {
            RestSharpHelper.Log = Log.Debug;
            HostAddress = new Lazy<string>(StartHosting);
            _defaultRequestFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory(HostAddress.Value));
            _adminConnection = new Lazy<CoreDockerClient>(() => CreateLoggedInRequest(AdminUser, AdminPassword));
            _guestConnection = new Lazy<CoreDockerClient>(() => CreateLoggedInRequest("Guest@Guest.com", "guest!"));
        }

        public CoreDockerClient AdminClient()
        {
            return _adminConnection.Value;
        }

        public CoreDockerClient GuestClient()
        {
            return _guestConnection.Value;
        }

        #region Private Methods

        private static string StartHosting()
        {
            var port = new Random().Next(9000, 9999);
            var address = $"http://localhost:{port}";
            Environment.SetEnvironmentVariable("OpenId__HostUrl", address);
            Environment.SetEnvironmentVariable("OpenId__UseReferenceTokens", "true"); //helps with testing on appveyor
            TestLoggingHelper.EnsureExists();
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices((context, collection) =>
                    collection.AddSingleton<ILoggerFactory>(services =>
                        new Serilog.Extensions.Logging.SerilogLoggerFactory()))
                .ConfigureAppConfiguration(Program.SettingsFileReaderHelper)
                .UseStartup<Startup>()
                .UseUrls(address);
            host.Build().Start();

            Log.Information($"Starting api on [{address}]");
            var forContext = Log.ForContext(typeof(RestSharpHelper));
            RestSharpHelper.Log = m => { forContext.Debug(m); };
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
            return (CoreDockerClient) _defaultRequestFactory.Value.GetConnection();
        }

        #endregion
    }
}