using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models.Auth;
using CoreDocker.Utilities.Helpers;
using GraphQL;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Serilog;

namespace CoreDocker.Sdk.RestApi
{
    public class CoreDockerClient : ICoreDockerClient
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private GraphQLHttpClient _graphQlClient;
        internal RestClient _restClient;

        public CoreDockerClient(string urlBase)
        {
            UrlBase = urlBase;
            _restClient = new RestClient(UrlBase);
            Authenticate = new AuthenticateApiClient(this);
            Projects = new ProjectApiClient(this);
            Users = new UserApiClient(this);
            Ping = new PingApiClient(this);
            _graphQlClient = GraphQlClient();
        }

        public RestClient Client => _restClient;

        public string UrlBase { get; }

        public async Task<GraphQLResponse<dynamic>> GraphQlPost(GraphQLRequest request)
        {
            var graphQlResponse = await _graphQlClient.SendQueryAsync<dynamic>(request);
            if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
                throw new GraphQlResponseException<dynamic>(graphQlResponse);
            return graphQlResponse;
        }


        public async Task<GraphQLResponse<T>> Post<T>(GraphQLRequest request)
        {
            try
            {
                var graphQlResponse = await _graphQlClient.SendQueryAsync<T>(request);
                if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
                    throw new GraphQlResponseException<T>(graphQlResponse);
                return graphQlResponse;
            }
            catch (GraphQLHttpRequestException e)
            {
                if (e.Content.Contains("errors"))
                {
                    var graphQlResponse = JsonConvert.DeserializeObject<GraphQLResponse<T>>(e.Content);
                    graphQlResponse.Dump("graphQlResponse");
                    
                    if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
                        throw new GraphQlResponseException<T>(graphQlResponse);
                }

                throw;
            }
        }

        public IObservable<GraphQLResponse<RealTimeEventResponse>> SendSubscribeGeneralEvents()
        {
            var request = new GraphQLRequest(@"subscription { onDefaultEvent{id,event,correlationId}}");
            return _graphQlClient.CreateSubscriptionStream<RealTimeEventResponse>(request);
        }

        public class RealTimeEventResponse
        {
            public RealTimeEvent OnDefaultEvent { get; set; }
        }

        #region Nested type: RealTimeEvent

        public class RealTimeEvent
        {
            public string Id { get; set; }
            public string Event { get; set; }
            public string CorrelationId { get; set; }
            public string Exception { get; set; }
        }

        #endregion

        #region Implementation of ICoreDockerApi

        public void SetToken(TokenResponseModel data)
        {
            var bearerToken = $"Bearer {data.AccessToken}";
            _restClient.DefaultParameters.Add(new Parameter("Authorization", bearerToken, ParameterType.HttpHeader));
            _graphQlClient = GraphQlClient(data.AccessToken);
        }

        private GraphQLHttpClient GraphQlClient(string dataAccessToken = null)
        {
            var jsonSerializer = new NewtonsoftJsonSerializer(settings => settings.ContractResolver =
                new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                });
            var graphQlHttpClientOptions = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(UrlBase.UriCombine("/graphql")),
                HttpMessageHandler = new WithAuthHeader(dataAccessToken)
                
            };
            return new GraphQLHttpClient(graphQlHttpClientOptions, jsonSerializer) ;
        }

        public class WithAuthHeader : HttpClientHandler
        {
            private readonly string _token;

            public WithAuthHeader(string token)
            {
                _token = token;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken token)
            {
                if (_token != null) request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token);

                return await base.SendAsync(request, token);
            }
        }


        public AuthenticateApiClient Authenticate { get; set; }
        public PingApiClient Ping { get; set; }


        public ProjectApiClient Projects { get; set; }
        public UserApiClient Users { get; set; }

        #endregion
    }
}