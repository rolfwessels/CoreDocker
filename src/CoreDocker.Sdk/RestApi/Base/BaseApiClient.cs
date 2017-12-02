using CoreDocker.Shared;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

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
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            FlurlHttp.Configure(x => x.JsonSerializer = new Flurl.Http.Configuration.NewtonsoftJsonSerializer(jsonSerializerSettings));
        }

        protected virtual Url DefaultUrl(string appendToUrl = null)
        {
            return new Url(CoreDockerClient.UrlBase.AppendUrl(_baseUrl).AppendUrl(appendToUrl));
        }

    }
}