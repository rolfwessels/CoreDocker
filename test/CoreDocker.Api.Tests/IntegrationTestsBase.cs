﻿using System;
using CoreDocker.Sdk;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CoreDocker.Api.Tests
{
    public class IntegrationTestsBase 
    {
        public const string ClientId = "CoreDocker.Api";
        public const string AdminPassword = "admin!";
        public const string AdminUser = "admin@admin.com";

        protected static Lazy<ConnectionFactory> _defaultRequestFactory;
        protected static Lazy<CoreDockerClient> _adminConnection;
        protected static Lazy<CoreDockerClient> _guestConnection;

        static IntegrationTestsBase()
        {
            RestSharpHelper.Log = Log.Debug;
            _defaultRequestFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory(()=>new HostBuilder().CreateClient()));
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

        class HostBuilder : WebApplicationFactory<Program>
        {
            protected override IHost CreateHost(IHostBuilder builder)
            {
                builder.ConfigureServices(services =>
                {
                    // add overrides here
                });

                return base.CreateHost(builder);
            }

        }
       
        


        private static CoreDockerClient CreateLoggedInRequest(string adminAdminCom, string adminPassword)
        {
            var api = _defaultRequestFactory.Value.GetConnection();
            api.Authenticate.Login(adminAdminCom, adminPassword).Wait();
            return (CoreDockerClient)api;
        }

        protected CoreDockerClient NewClientNotAuthorized()
        {
            return (CoreDockerClient)_defaultRequestFactory.Value.GetConnection();
        }
    }
}