using CoreDocker.Shared;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreDocker.Sdk
{
    public class ApiClientBase
    {
        protected CoreDockerClient CoreDockerClient;
        private readonly string _baseUrl;

        public ApiClientBase(CoreDockerClient coreDockerClient, string baseUrl)
        {
            CoreDockerClient = coreDockerClient;
            _baseUrl = baseUrl;
            var jsonSerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Objects,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            FlurlHttp.Configure(x => x.JsonSerializer = new NewtonsoftJsonSerializer(jsonSerializerSettings));
        }
        
        protected virtual Url DefaultUrl(string appendToUrl = null)
        {
            return new Url(CoreDockerClient.UrlBase.AppendUrl(_baseUrl).AppendUrl(appendToUrl));
        }
    }
}