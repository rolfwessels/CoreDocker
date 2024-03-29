﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Shared;
using RestSharp;

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

        public async Task<PagedResult<TReferenceModel>> GetPaged(string oDataQuery)
        {
            var restRequest = new RestRequest(Url($"?{EnsureHasInlinecount(oDataQuery)}"));
            var executeAsyncWithLogging =
                await CoreDockerClient.Client.ExecuteAsyncWithLogging<PagedResult<TReferenceModel>>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<PagedResult<TModel>> GetDetailPaged(string oDataQuery)
        {
            var restRequest =
                new RestRequest(Url($"{RouteHelper.WithDetail}?{EnsureHasInlinecount(oDataQuery)}"));
            var executeAsyncWithLogging =
                await CoreDockerClient.Client.ExecuteAsyncWithLogging<PagedResult<TModel>>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<IEnumerable<TReferenceModel>> Get(string oDataQuery)
        {
            var restRequest = new RestRequest(Url($"?{oDataQuery}"));
            var executeAsyncWithLogging =
                await CoreDockerClient.Client.ExecuteAsyncWithLogging<List<TReferenceModel>>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<IEnumerable<TModel>> GetDetail(string oDataQuery)
        {
            var restRequest = new RestRequest(Url($"{RouteHelper.WithDetail}?{oDataQuery}"));
            var executeAsyncWithLogging =
                await CoreDockerClient.Client.ExecuteAsyncWithLogging<List<TModel>>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        private static string EnsureHasInlinecount(string oDataQuery)
        {
            if (oDataQuery == null || !oDataQuery.Contains("$inlinecount"))
            {
                oDataQuery = $"{oDataQuery}&$inlinecount=allpages";
            }

            return oDataQuery;
        }

        public Task<IEnumerable<TReferenceModel>> Get()
        {
            return Get("");
        }

        public Task<IEnumerable<TModel>> GetDetail()
        {
            return GetDetail("");
        }
    }
}