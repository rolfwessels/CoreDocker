using System;
using System.Net;
using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Shared;
using CoreDocker.Shared.Models;
using RestSharp;

namespace CoreDocker.Sdk.RestApi.Base
{
    public  abstract class BaseApiClient
    {
        protected CoreDockerClient CoreDockerClient;
        private readonly string _baseUrl;

        protected BaseApiClient(CoreDockerClient coreDockerClient, string baseUrl)
        {
            CoreDockerClient = coreDockerClient;
            _baseUrl = baseUrl;
            
        }

        protected virtual string DefaultUrl(string appendToUrl = null)
        {
            return CoreDockerClient.UrlBase.AppendUrl(_baseUrl).AppendUrl(appendToUrl);
        }

        protected virtual string DefaultTokenUrl(string appendToUrl = null)
        {
            return CoreDockerClient.UrlBase.AppendUrl(_baseUrl).AppendUrl(appendToUrl);
        }

        protected virtual T ValidateResponse<T>(IRestResponse<T> result)
        {
            if (result.StatusCode != HttpStatusCode.OK)
            {
                if (string.IsNullOrEmpty(result.Content)) throw new ApplicationException(
                    $"{result.StatusCode} response contains no data.");
                Console.Out.WriteLine("SimpleJson.SimpleJson.CurrentJsonSerializerStrategy.DeserializeObject(result.Content,typeof(ErrorMessage))+"+ SimpleJson.SimpleJson.CurrentJsonSerializerStrategy.DeserializeObject(result.Content, typeof(ErrorMessage)));
                var errorMessage = SimpleJson.SimpleJson.DeserializeObject<ErrorMessage>(result.Content);
                throw new Exception(errorMessage.Message);
            }
            return result.Data;
        }

        protected async Task<T> ExecuteAndValidate<T>(RestRequest request) where T : new()
        {
            var response = await CoreDockerClient.Client.ExecuteAsyncWithLogging<T>(request);
            ValidateResponse(response);
            return response.Data;
        }

        protected async Task<bool> ExecuteAndValidateBool(RestRequest request)
        {
            var response = await CoreDockerClient.Client.ExecuteAsyncWithLogging<bool>(request);
            ValidateResponse(response);
            return Convert.ToBoolean(response.Content);
        }
    }
}