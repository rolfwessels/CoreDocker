using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models.Shared;
using RestSharp;
using RestSharp.Serializers;

namespace CoreDocker.Sdk.RestApi.Base
{
    public class BaseCrudApiClient<TModel, TDetailModel, TReferenceModel> : BaseGetApiClient<TModel, TReferenceModel>,
        ICrudController<TModel, TDetailModel>
        where TModel : IBaseModel, new()
    {
        protected BaseCrudApiClient(CoreDockerClient dockerClient, string baseUrl)
            : base(dockerClient, baseUrl)
        {
        }

        public async Task<TModel> GetById(string id)
        {
            var restRequest = new RestRequest(DefaultUrl(RouteHelper.WithId.SetParam("id", id)));
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<TModel>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<TModel> Insert(TDetailModel model)
        {
            var restRequest = new RestRequest(DefaultUrl(), Method.Post);
            restRequest.AddBody(model!, ContentType.Json);
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<TModel>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<TModel> Update(string id, TDetailModel model)
        {
            var restRequest = new RestRequest(DefaultUrl(RouteHelper.WithId.SetParam("id", id)), Method.Put);
            restRequest.AddBody(model!, ContentType.Json);
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<TModel>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<bool> Delete(string id)
        {
            var restRequest = new RestRequest(DefaultUrl(RouteHelper.WithId.SetParam("id", id)), Method.Delete);
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<bool>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }
    }
}