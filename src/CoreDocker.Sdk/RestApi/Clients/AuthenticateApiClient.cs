using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared.Models;
using RestSharp;

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class AuthenticateApiClient : BaseApiClient
    {
        private readonly CoreDockerClient _coreDockerClient;

        public AuthenticateApiClient(CoreDockerClient coreDockerClient) : base(coreDockerClient, "connect/token")
        {
            _coreDockerClient = coreDockerClient;
        }

        public async Task<Jwks> GetConfigAsync()
        {
            var restRequest = new RestRequest(".well-known/openid-configuration/jwks");
            var restRequestAsyncHandle = await _coreDockerClient.Client.ExecuteAsyncWithLogging<Jwks>(restRequest);
            return restRequestAsyncHandle.Data;
        }

        public class Jwks
        {
            public List<Dictionary<string, string>> Keys { get; set; }
        }

        public Task<TokenResponseModel> GetToken(string client, string adminUser, string adminPassword)
        {
            return GetToken(new TokenRequestModel()
            {
                ClientId = client,
                UserName = adminUser,
                Password = adminPassword
            });
        }

        public async Task<TokenResponseModel> GetToken(TokenRequestModel tokenRequestModel)
        {
            var request = new RestRequest(DefaultUrl(), Method.POST);
//            request.AddHeader("Authorization", "Basic YXBpOjUxYmUyMTNiMzQwNzQ3NWVhMDlkMTY5OGFhZjZlZDE1");
            request.AddParameter("username", tokenRequestModel.UserName);
            request.AddParameter("password", tokenRequestModel.Password);
            request.AddParameter("grant_type", tokenRequestModel.GrantType);
//            request.AddParameter("scope", tokenRequestModel.Scope);
            
            IRestResponse<TokenResponseModel> result = await _coreDockerClient.Client.ExecuteAsyncWithLogging<TokenResponseModel>(request);
            ValidateResponse(result);
//            var bearerToken = string.Format("{0} {1}", result.Data.TokenType.ToInitialCase(), result.Data.AccessToken);
//            _restClient.DefaultParameters.Add(new Parameter() { Type = ParameterType.HttpHeader, Name = "Authorization", Value = bearerToken });
            return result.Data;
        }
    }

    
}