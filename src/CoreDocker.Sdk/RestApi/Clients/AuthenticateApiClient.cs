using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared.Models.Auth;
using Newtonsoft.Json;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class AuthenticateApiClient : BaseApiClient
    {
        private readonly CoreDockerClient _coreDockerClient;

        public AuthenticateApiClient(CoreDockerClient coreDockerClient) : base(coreDockerClient, "connect")
        {
            _coreDockerClient = coreDockerClient;
        }

        public async Task<Jwks> GetConfigAsync()
        {
            var restRequest = new RestRequest(".well-known/openid-configuration/jwks");
            var restRequestAsyncHandle = await _coreDockerClient.Client.ExecuteAsync(restRequest);
            return restRequestAsyncHandle.Content != null
                ? JsonConvert.DeserializeObject<Jwks>(restRequestAsyncHandle.Content)
                : new Jwks();
        }

        public async Task<TokenResponseModel> Login(string adminUser, string adminPassword)
        {
            var token = await GetToken(new TokenRequestModel(ClientId: "coredocker.api",
                ClientSecret: "super_secure_password", UserName: adminUser, Password: adminPassword));
            CoreDockerClient.SetToken(token);
            return token;
        }

        public async Task<TokenResponseModel> GetToken(TokenRequestModel tokenRequestModel)
        {
            var request = new RestRequest(DefaultTokenUrl("token"), Method.Post);
            request.AddParameter("client_id", tokenRequestModel.ClientId);
            request.AddParameter("client_secret", tokenRequestModel.ClientSecret);
            request.AddParameter("username", tokenRequestModel.UserName);
            request.AddParameter("password", tokenRequestModel.Password);
            request.AddParameter("grant_type", tokenRequestModel.GrantType);
            request.AddParameter("scope", "api");

            var result = await _coreDockerClient.Client.ExecuteAsyncWithLogging<TokenResponseModel>(request);
            ValidateTokenResponse(result);
            return ValidateResponse(result);
        }

        protected virtual void ValidateTokenResponse<T>(RestResponse<T> result)
        {
            if (result.StatusCode != HttpStatusCode.OK)
            {
                if (string.IsNullOrEmpty(result.Content))
                {
                    throw new ApplicationException(
                        $"{result.StatusCode} response contains no data.");
                }

                var errorMessage = JsonSerializer.Deserialize<TokenErrorMessage>(result.Content)!;
                throw new Exception($"{errorMessage.Error}[{errorMessage.ErrorDescription}]");
            }
        }

        public class Jwks
        {
            [JsonPropertyName("keys")] public List<Dictionary<string, object>> Keys { get; set; } = new();
        }


        internal class TokenErrorMessage
        {
            [JsonPropertyName("error")] public string? Error { get; set; }

            [JsonPropertyName("error_description")]
            public string? ErrorDescription { get; set; }
        }
    }
}