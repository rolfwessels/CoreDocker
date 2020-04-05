using System;
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
using GraphQL.Client.Http;
using RestSharp;
using Serilog;
using GraphQL.Client.Serializer.Newtonsoft;

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
            _graphQlClient = new GraphQLHttpClient(UrlBase.UriCombine("/graphql"), new NewtonsoftJsonSerializer()) ;
            
        }

        public RestClient Client => _restClient;

        public string UrlBase { get; }

        public async Task<GraphQLResponse<dynamic>> GraphQlPost(GraphQLRequest heroRequest)
        {
            var graphQlResponse = await _graphQlClient.SendQueryAsync<dynamic>(heroRequest);
            if (graphQlResponse.Errors != null && graphQlResponse.Errors.Any())
            {
                throw new GraphQlResponseException(graphQlResponse);
            }
            return graphQlResponse;
        }

        public IObservable<GraphQLResponse<RealTimeEvent>> SendSubscribeGeneralEvents()
        {
            var request = new GraphQLRequest(@"subscription { onDefaultEvent{id,event,correlationId}}");
            return _graphQlClient.CreateSubscriptionStream<RealTimeEvent>(request);
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

        #region Nested type: SafeDisposeWrapper

        public class SafeDisposeWrapper : IDisposable
        {
            private static readonly ILogger Logger = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
            private readonly IDisposable _disposable;

            public SafeDisposeWrapper(IDisposable disposable)
            {
                _disposable = disposable;
            }

            #region IDisposable Members

            public void Dispose()
            {
                try
                {
                    _disposable.Dispose();
                }
                catch (Exception e)
                {
                    Logger.Error($"SafeDisposeWrapper:Dispose {e.Message}");
                }
            }

            #endregion
        }

        #endregion

        #region Implementation of ICoreDockerApi

        public void SetToken(TokenResponseModel data)
        {
            var bearerToken = $"Bearer {data.AccessToken}";
            _restClient.DefaultParameters.Add(new Parameter("Authorization", bearerToken, ParameterType.HttpHeader));
            _graphQlClient = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = new Uri(UrlBase.UriCombine("/graphql")), HttpMessageHandler = new MyHandler(data.AccessToken)
            }, new NewtonsoftJsonSerializer());
        }

        public class MyHandler : DelegatingHandler
        {
            private readonly AuthenticationHeaderValue _authentication;

            public MyHandler(string token, HttpMessageHandler inner = null) : base(inner ?? new HttpClientHandler())
            {
                _authentication = new AuthenticationHeaderValue("bearer", token);
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Authorization = _authentication;
                return await base.SendAsync(request, cancellationToken);
            }
        }


        public AuthenticateApiClient Authenticate { get; set; }
        public PingApiClient Ping { get; set; }


        public ProjectApiClient Projects { get; set; }
        public UserApiClient Users { get; set; }

        #endregion
    }
}