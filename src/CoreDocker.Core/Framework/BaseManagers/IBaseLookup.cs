using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.BaseManagers
{
    public interface IBaseLookup<T>
    {
        Task<List<T>> Get();
        Task<T?> GetById(string id);
    }
}