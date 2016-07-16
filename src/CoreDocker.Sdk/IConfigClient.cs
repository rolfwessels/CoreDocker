using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Shared.Dto;

namespace CoreDocker.Sdk
{
    public interface IConfigClient
    {
        Task<List<Config>> Get();
        Task<Config> Get(string id);
        Task<Config> Insert(Config entity);
        Task<Config> Update(string id, Config entity);
        Task<Config> Delete(string id);
    }
}