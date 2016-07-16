using System;
using Flurl.Http;

namespace CoreDocker.Sdk
{
    public class ConnectionFactory
    {
        private readonly string _urlBase;

        public ConnectionFactory(string urlBase)
        {
            FlurlHttp.Configure(c => { c.DefaultTimeout = TimeSpan.FromSeconds(30); });
            _urlBase = urlBase;
        }

        public ICoreDockerApi GetConnection()
        {
            return new CoreDockerClient(_urlBase);
        }
    }
}