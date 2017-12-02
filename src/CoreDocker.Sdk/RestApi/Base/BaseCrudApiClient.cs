using System.Threading.Tasks;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models.Interfaces;
using CoreDocker.Sdk;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Shared;
using Flurl.Http;

namespace CoreDocker.Sdk.RestApi.Base
{
    public class BaseCrudApiClient<TModel, TDetailModel, TReferenceModel> : BaseGetApiClient<TModel, TReferenceModel>,
                                                                            ICrudController<TModel, TDetailModel>
        where TModel : IBaseModel, new()
    {
        protected BaseCrudApiClient(CoreDockerClient dockerClient, string baseUrl)
            : base(dockerClient, baseUrl) { 

        }

        #region ICrudController<TModel,TDetailModel> Members

        public Task<TModel> GetById(string id)
        {
            var receiveJson = DefaultUrl(RouteHelper.WithId.SetParam("id", id))
                .GetAsyncAndLog()
                .ReceiveJson<TModel>();
            return receiveJson;
        }

        public async Task<TModel> Insert(TDetailModel model)
        {
            return await DefaultUrl()
              .PostJsonAsyncAndLog(model)
              .ReceiveJson<TModel>();
        }

        public async Task<TModel> Update(string id, TDetailModel model)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
            .PutJsonAsyncAndLog(model)
            .ReceiveJson<TModel>();
        }

        public async Task<bool> Delete(string id)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
                .DeleteAsyncAndLog()
                .ReceiveJson<bool>();
        }

        #endregion
    }
}