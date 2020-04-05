using CoreDocker.Sdk.RestApi;

namespace CoreDocker.Sdk
{
    public class ConnectionFactory
    {
        private readonly string _urlBase;

        public ConnectionFactory(string urlBase)
        {
            _urlBase = urlBase;
        }

        public ICoreDockerClient GetConnection()
        {
            return new CoreDockerClient(_urlBase);
        }
    }
}