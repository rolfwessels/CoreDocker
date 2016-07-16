using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Shared;
using CoreDocker.Shared.Dto;
using Flurl.Http;

namespace CoreDocker.Sdk
{
    public class ConfigClient : ApiClientBase, IConfigClient
    {
        public ConfigClient(CoreDockerClient coreDockerClient)
            : base(coreDockerClient, RouteHelper.Config)
        {
        }
        
        #region Implementation of IConfigClient

        public async Task<List<Config>> Get()
        {
            return await DefaultUrl()
                .GetAsyncAndLog()
                .ReceiveJson<List<Config>>();
        }
        
        public async Task<Config> Get(string id)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
               .GetAsyncAndLog()
               .ReceiveJson<Config>();
        }

        public async Task<Config> Insert(Config entity)
        {
            return await DefaultUrl()
                .PutJsonAsyncAndLog(entity)
                .ReceiveJson<Config>();
        }

        public async Task<Config> Update(string id, Config entity)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
                .PostJsonAsyncAndLog(entity)
                .ReceiveJson<Config>();
        }

        public async Task<Config> Delete(string id)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
                .DeleteAsyncAndLog()
                .ReceiveJson<Config>();
        }

        public Task<dynamic> GetData(string id)
        {
            return DefaultUrl(RouteHelper.WithId.SetParam("id", id))
                .AppendPathSegment("data")
                .GetAsyncAndLog()
                .ReceiveJson<dynamic>();
        }

        #endregion
    }

    
}