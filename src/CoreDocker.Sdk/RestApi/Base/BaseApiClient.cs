using Newtonsoft.Json;
using CoreDocker.Sdk;
using Flurl.Http;
using Flurl;
using CoreDocker.Shared;

namespace MainSolutionTemplate.Sdk.OAuth
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
                TypeNameHandling = TypeNameHandling.Objects,
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