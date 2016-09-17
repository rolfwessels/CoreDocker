using System;
using System.Threading.Tasks;

namespace CoreDocker.Shared.Interfaces.Base
{
    public interface ICrudController<TModel, TDetailModel> 
    {
        Task<TModel> GetById(string id);
        Task<TModel> Insert(TDetailModel model);
        Task<TModel> Update(string id, TDetailModel model);
        Task<bool> Delete(string id);
    }
}