using System.Threading.Tasks;

namespace CoreDocker.Shared.Interfaces.Base
{
    public interface ICrudController<TModel, in TDetailModel>
    {
        Task<TModel> GetById(string id);
    }
}