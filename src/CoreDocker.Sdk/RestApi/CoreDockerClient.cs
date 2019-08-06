using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models.Auth;
using CoreDocker.Utilities.Helpers;
using GraphQL.Client;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using RestSharp;
using Serilog;

namespace CoreDocker.Sdk.RestApi
{
    public class CoreDockerClient : ICoreDockerClient
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
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

        private void SubscriptionCallback(GraphQLResponse obj, Action<GraphQLResponse> callback)
        {
            if (obj == null)
            {
                _log.Error($"CoreDockerClient:SubscriptionCallback Subscription value is null");
            }
            else
            {
                callback(obj);
            }
        }

        public async Task<IDisposable> SendSubscribeGeneralEventsAsync(Action<RealTimeEvent, dynamic> callback)
        {
#pragma warning disable 618
            IGraphQLSubscriptionResult subscriptionResult = await _graphQlClient.SendSubscribeAsync(@"subscription { onDefaultEvent{id,event,correlationId}}");
#pragma warning restore 618
            subscriptionResult.OnReceive += response1 => SubscriptionCallback(response1, response =>
            {
                _log.Debug("***** something +" + response);
                var dynamicCastTo = CastHelper.DynamicCastTo<RealTimeEvent>(response.Data.generalEvents);
                callback(dynamicCastTo, response);
            });
            return new SafeDisposeWrapper(subscriptionResult);
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
    }
}