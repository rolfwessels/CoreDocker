using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models;
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
            Ping = new PingApiClient(this);
        }


        public string UrlBase { get; }

        #region Implementation of ICoreDockerApi

        public void SetToken(TokenResponseModel data)
        {
            var bearerToken = string.Format("{0} {1}", "Bearer", data.AccessToken);
            _restClient.DefaultParameters.Add(new Parameter() { Type = ParameterType.HttpHeader, Name = "Authorization", Value = bearerToken });
        }

        public AuthenticateApiClient Authenticate { get; set; }
        public PingApiClient Ping { get; set; }
       

        public ProjectApiClient Projects { get; set; }
        public UserApiClient Users { get; set; }

        #endregion


        public RestClient Client => _restClient;
    }
}