using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.DataIntegrity
{
    public interface IDataIntegrityManager
    {
        Task<long> UpdateAllReferences<T>(T updatedValue);
        Task<long> GetReferenceCount<T>(T updatedValue);
    }
}