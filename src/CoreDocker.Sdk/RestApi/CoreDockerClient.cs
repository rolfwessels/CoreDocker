using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models.Auth;
using GraphQL;
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
        private readonly HttpClient _sharedClient;
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private GraphQLHttpClient _graphQlClient;
        internal RestClient _restClient;

        public CoreDockerClient(string urlBase) : this(new HttpClient { BaseAddress = new Uri(urlBase) })
        {
        }

        public CoreDockerClient(HttpClient sharedClient)
        {
            _sharedClient = sharedClient;
            _restClient = new RestClient(sharedClient);
            Authenticate = new AuthenticateApiClient(this);
            Projects = new ProjectApiClient(this);
            Users = new UserApiClient(this);
            Ping = new PingApiClient(this);
            _graphQlClient = GraphQlClient();
        }

        public RestClient Client => _restClient;


        public AuthenticateApiClient Authenticate { get; set; }
        public PingApiClient Ping { get; set; }


        public ProjectApiClient Projects { get; set; }
        public UserApiClient Users { get; set; }

        public async Task<GraphQLResponse<dynamic>> GraphQlPost(GraphQLRequest request)
        {
            var graphQlResponse = await _graphQlClient.SendQueryAsync<dynamic>(request);
            if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
            {
                throw new GraphQlResponseException<dynamic>(graphQlResponse);
            }

            return graphQlResponse;
        }


        public async Task<GraphQLResponse<T>> Post<T>(GraphQLRequest request)
        {
            try
            {
                var graphQlResponse = await _graphQlClient.SendQueryAsync<T>(request);
                if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
                {
                    throw new GraphQlResponseException<T>(graphQlResponse);
                }

                return graphQlResponse;
            }
            catch (GraphQLHttpRequestException e)
            {
                if (e.Content != null && e.Content.Contains("errors"))
                {
                    var graphQlResponse = JsonConvert.DeserializeObject<GraphQLResponse<T>>(e.Content);
                    if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
                    {
                        throw new GraphQlResponseException<T>(graphQlResponse);
                    }
                }

                throw;
            }
        }

        public IObservable<GraphQLResponse<RealTimeEventResponse>> SendSubscribeGeneralEvents()
        {
            var request = new GraphQLRequest(@"subscription { onDefaultEvent{id,event,correlationId}}");
            return _graphQlClient.CreateSubscriptionStream<RealTimeEventResponse>(request,x=>_log.Error(x,"Broken"));
        }

        public record RealTimeEventResponse(RealTimeEvent OnDefaultEvent);

        public record RealTimeEvent(string Id, string Event, string CorrelationId, string Exception);

        public void SetToken(TokenResponseModel data)
        {
            var bearerToken = $"Bearer {data.AccessToken}";
            _restClient.AddDefaultParameter("Authorization", bearerToken, ParameterType.HttpHeader);
            _graphQlClient = GraphQlClient(data.AccessToken);
        }

        private GraphQLHttpClient GraphQlClient(string? dataAccessToken = null)
        {
            var jsonSerializer = new NewtonsoftJsonSerializer(settings => settings.ContractResolver =
                new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                });
            var endPoint = new Uri(_sharedClient.BaseAddress??new Uri("http://localhost/"),"graphql");
            var webSocketEndPoint = endPoint.Scheme.Equals("https") ? new Uri(endPoint.ToString().Replace("https://", "wss://")) : new Uri(endPoint.ToString().Replace("http://", "ws://"));
            
            var graphQlHttpClientOptions = new GraphQLHttpClientOptions
            {
                EndPoint = endPoint,
                WebSocketEndPoint =  webSocketEndPoint
            };
            _sharedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", dataAccessToken);
            return new GraphQLHttpClient(graphQlHttpClientOptions, jsonSerializer,_sharedClient);
        }

    }
}