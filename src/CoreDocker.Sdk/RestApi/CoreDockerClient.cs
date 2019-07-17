using System;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models.Auth;
using CoreDocker.Utilities.Helpers;
using GraphQL.Client;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using RestSharp;

namespace CoreDocker.Sdk.RestApi
{
    public class CoreDockerClient : ICoreDockerClient
    {
        private readonly GraphQLHttpClient _graphQlClient;
        internal RestClient _restClient;

        public CoreDockerClient(string urlBase)
        {
            UrlBase = urlBase;
            _restClient = new RestClient(UrlBase);
            Authenticate = new AuthenticateApiClient(this);
            Projects = new ProjectApiClient(this);
            Users = new UserApiClient(this);
            Ping = new PingApiClient(this);
            _graphQlClient = new GraphQLHttpClient(UrlBase.UriCombine("/graphql"));
        }

        public RestClient Client => _restClient;

        public string UrlBase { get; }

        public async Task<GraphQLResponse> GraphQlPost(GraphQLRequest heroRequest)
        {
            var graphQlResponse = await _graphQlClient.SendQueryAsync(heroRequest);
            if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
            {
                graphQlResponse.Dump("graphQlResponse");
                throw new GraphQlResponseException(graphQlResponse);
            }

            return graphQlResponse;
        }

        #region Implementation of ICoreDockerApi

        public void SetToken(TokenResponseModel data)
        {
            var bearerToken = $"Bearer {data.AccessToken}";
            _restClient.DefaultParameters.Add(new Parameter
            {
                Type = ParameterType.HttpHeader,
                Name = "Authorization",
                Value = bearerToken
            });
            _graphQlClient.DefaultRequestHeaders.Add("Authorization", new[] {bearerToken});
        }

        public AuthenticateApiClient Authenticate { get; set; }
        public PingApiClient Ping { get; set; }


        public ProjectApiClient Projects { get; set; }
        public UserApiClient Users { get; set; }

        #endregion

        public async Task<IDisposable> SendSubscribeAsync(string query, Action<GraphQLResponse> callback)
        {
#pragma warning disable 618
            var subscriptionResult = await _graphQlClient.SendSubscribeAsync(query);
#pragma warning restore 618
            subscriptionResult.OnReceive += callback;
            return subscriptionResult;
        }

        public Task<IDisposable> SendSubscribeGeneralEventsAsync(Action<RealTimeEvent, dynamic> callback)
        {
            return SendSubscribeAsync(@"subscription { generalEvents{id,event,correlationId}}", response =>
            {
                var dynamicCastTo = CastHelper.DynamicCastTo<RealTimeEvent>(response.Data.generalEvents);
                callback(dynamicCastTo, response);
            });
        }

        public class RealTimeEvent
        {
            public string Id { get; set; }
            public string Event { get; set; }
            public string CorrelationId { get; set; }
            public string Exception { get; set; }
        }
    }
}