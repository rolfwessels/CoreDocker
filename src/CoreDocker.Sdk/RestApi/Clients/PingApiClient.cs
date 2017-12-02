using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Models;
using RestSharp;

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class PingApiClient : BaseApiClient
    {
        public PingApiClient(CoreDockerClient coreDockerClient) : base(coreDockerClient, "ping")
        {
        }

        public async Task<PingModel> Get()
        {
            var restRequest = new RestRequest(RouteHelper.PingController);
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<PingModel>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }
    }
}