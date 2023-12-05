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
        public async Task<string?> GetConfiguration()
        {
            var restRequest = new RestRequest(".well-known/openid-configuration");
            var restRequestAsyncHandle = await _coreDockerClient.Client.ExecuteAsync(restRequest);
            return restRequestAsyncHandle.Content;
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
            var request = new RestRequest(Url("token"), Method.Post);
            request.AddParameter("client_id", tokenRequestModel.ClientId);
            request.AddParameter("client_secret", tokenRequestModel.ClientSecret);
            request.AddParameter("username", tokenRequestModel.UserName);
            request.AddParameter("password", tokenRequestModel.Password);
            request.AddParameter("grant_type", tokenRequestModel.GrantType);
            request.AddParameter("scope", "api openid email");

            var result = await _coreDockerClient.Client.ExecuteAsyncWithLogging<TokenResponseModel>(request);
            ValidateTokenResponse(result);
            return ValidateResponse(result);
        }

        public async Task<UserInfoResponse> UserInfo()
        {
            var request = new RestRequest(Url("userinfo"));
            var result = await _coreDockerClient.Client.ExecuteAsyncWithLogging<UserInfoResponse>(request);
            ValidateTokenResponse(result);
            return ValidateResponse(result);
        }

        public record UserInfoResponse(string Sub, string Email, string Name, string GivenName);
        //
        // public async Task<bool> Logout()
        // {
        //     var token = _coreDockerClient.GetToken();
        //     if (token == null)
        //     {
        //         throw new Exception("No token found");
        //     }
        //
        //     var request = new RestRequest(Url("revocation"), Method.Post);
        //     request.AddParameter("client_id", _clientId);
        //     request.AddParameter("client_secret", _secret);
        //     request.AddParameter("token", DataVisualisationClient.GetToken()?.AccessToken);
        //
        //     var result = await DataVisualisationClient.Client.ExecuteAsyncWithLogging<TokenResponseModel>(request);
        //     ValidateTokenResponse(result);
        //     return result.StatusCode == HttpStatusCode.OK;
        // }

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