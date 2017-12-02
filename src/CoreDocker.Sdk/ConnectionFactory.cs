using System;
using CoreDocker.Sdk.RestApi;
using Flurl.Http;

namespace CoreDocker.Sdk
{
    public class ConnectionFactory
    {
        private readonly string _urlBase;

        public ConnectionFactory(string urlBase)
        {
            FlurlHttp.Configure(c => { c.Timeout = TimeSpan.FromSeconds(30); });
            _urlBase = urlBase;
        }

        public ICoreDockerApi GetConnection()
        {
            return new CoreDockerClient(_urlBase);
        }
    }
}