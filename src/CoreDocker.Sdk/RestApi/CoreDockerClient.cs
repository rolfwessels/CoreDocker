using CoreDocker.Sdk.RestApi.Clients;
using Flurl.Http;
using RestSharp;

namespace CoreDocker.Sdk.RestApi
{
    public class CoreDockerClient : ICoreDockerApi
    {
        internal RestClient _restClient;

        public CoreDockerClient(string urlBase)
        {
            UrlBase = urlBase;
            _restClient = new RestClient(UrlBase);
            Authenticate = new AuthenticateApiClient(this);
            Projects = new ProjectApiClient(this);
            Users = new UserApiClient(this);
        }


        public string UrlBase { get; }

        #region Implementation of ICoreDockerApi
        
        public AuthenticateApiClient Authenticate { get; set; }
        public ProjectApiClient Projects { get; set; }
        public UserApiClient Users { get; set; }

        #endregion


        public RestClient Client => _restClient;
    }
}