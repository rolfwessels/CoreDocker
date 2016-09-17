using System.Threading.Tasks;
using CoreDocker.Dal.Models;

namespace CoreDocker.Core.DataIntegrity
{
    public interface IDataIntegrityManager
    {
        Task<long> UpdateAllReferences<T>(T updatedValue);
        Task<long> GetReferenceCount<T>(T updatedValue);
    }
}