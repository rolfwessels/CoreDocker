using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

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

        public Task<TokenResponseModel> GetToken(string adminUser, string adminPassword)
        {
            return GetToken(new TokenRequestModel()
            {
                ClientId = "coredocker.api",
                ClientSecret = "e0acca78-4dc2-46c6-83c6-c6aeacfffd46",
                UserName = adminUser,
                Password = adminPassword
            });
        }

        public async Task<TokenResponseModel> GetToken(TokenRequestModel tokenRequestModel)
        {
            var request = new RestRequest(DefaultUrl(), Method.POST);
            request.AddParameter("client_id", tokenRequestModel.ClientId);
            request.AddParameter("client_secret", tokenRequestModel.ClientSecret);
            request.AddParameter("username", tokenRequestModel.UserName);
            request.AddParameter("password", tokenRequestModel.Password);
            request.AddParameter("grant_type", tokenRequestModel.GrantType);
            request.AddParameter("scope", "api");
            var restClient = _coreDockerClient.Client;
            IRestResponse<TokenResponseModel> result =
                await restClient.ExecuteAsyncWithLogging<TokenResponseModel>(request);
            ValidateTokenResponse(result);
            return result.Data;
        }

        protected virtual void ValidateTokenResponse<T>(IRestResponse<T> result)
        {
            if (result.StatusCode != HttpStatusCode.OK)
            {
                if (string.IsNullOrEmpty(result.Content))
                    throw new ApplicationException(
                        $"{result.StatusCode} response contains no data.");
                var errorMessage = JsonConvert.DeserializeObject<TokenErrorMessage>(result.Content);
                throw new Exception($"{errorMessage.Error}[{errorMessage.error_description}]");
            }
        }

        internal class TokenErrorMessage
        {
            public string Error { get; set; }
            public string error_description { get; set; }
        }
    }
}