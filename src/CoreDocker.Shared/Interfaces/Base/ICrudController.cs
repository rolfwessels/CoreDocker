using System;
using System.Threading.Tasks;

namespace MainSolutionTemplate.Shared.Interfaces.Base
{
    public interface ICrudController<TModel, TDetailModel> 
    {
        Task<TModel> Get(string id);
        Task<TModel> Insert(TDetailModel model);
        Task<TModel> Update(string id, TDetailModel model);
        Task<bool> Delete(string id);
    }
}