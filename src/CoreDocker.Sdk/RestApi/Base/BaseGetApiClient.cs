using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.OAuth;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Interfaces;
using Flurl.Http;

namespace CoreDocker.Sdk.RestApi.Base
{
    public class BaseGetApiClient<TModel, TReferenceModel> : BaseApiClient,
                                                             IBaseStandardLookups<TModel, TReferenceModel>
        where TModel : IBaseModel, new()
    {
        public BaseGetApiClient(CoreDockerClient dockerClient, string baseUrl)
            : base(dockerClient, baseUrl)
        {
        }

        #region Implementation of IBaseStandardLookups<UserModel,UserReferenceModel>

        public Task<IEnumerable<TReferenceModel>> Get()
        {
            return Get("");
        }

        public Task<IEnumerable<TModel>> GetDetail()
        {
            return GetDetail("");
        }

        #endregion

        public async Task<PagedResult<TReferenceModel>> GetPaged(string oDataQuery)
        {
            return await DefaultUrl("?" + EnsureHasInlinecount(oDataQuery))
               .GetAsyncAndLog()
               .ReceiveJson<PagedResult<TReferenceModel>>();
        }

        public async Task<PagedResult<TModel>> GetDetailPaged(string oDataQuery)
        {
            return await DefaultUrl(RouteHelper.WithDetail+"?" + EnsureHasInlinecount(oDataQuery))
                .GetAsyncAndLog()
                .ReceiveJson<PagedResult<TModel>>();
        }

        public async Task<IEnumerable<TReferenceModel>> Get(string oDataQuery)
        {
            return await DefaultUrl("?" + oDataQuery)
             .GetAsyncAndLog()
             .ReceiveJson<List<TReferenceModel>>();
        }

        public async Task<IEnumerable<TModel>> GetDetail(string oDataQuery)
        {
            return await DefaultUrl(RouteHelper.WithDetail+"?" + oDataQuery)
            .GetAsyncAndLog()
            .ReceiveJson<List<TModel>>();
        }

        private static string EnsureHasInlinecount(string oDataQuery)
        {
            if (oDataQuery == null || !oDataQuery.Contains("$inlinecount"))
                oDataQuery = string.Format("{0}&$inlinecount=allpages", oDataQuery);
            return oDataQuery;
        }
    }
}