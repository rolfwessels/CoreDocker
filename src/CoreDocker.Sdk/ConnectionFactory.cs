using System;
using System.Net.Http;
using CoreDocker.Sdk.RestApi;

namespace CoreDocker.Sdk
{
    public class ConnectionFactory
    {
        private readonly Func<HttpClient> _createClient;
        private readonly SocketsHttpHandler? _socketHandler;

        public ConnectionFactory(string urlBase)
        {
            _socketHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(15) };
            _createClient = () =>
            {
                var httpClient = new HttpClient(_socketHandler)
                {
                    BaseAddress = new Uri(urlBase)
                };
                return httpClient;
            };
        }


        public ConnectionFactory(Func<HttpClient> createClient)
        {
            _createClient = createClient;
        }

        public ICoreDockerClient GetConnection()
        {
            return new CoreDockerClient(_createClient());
        }
    }
}