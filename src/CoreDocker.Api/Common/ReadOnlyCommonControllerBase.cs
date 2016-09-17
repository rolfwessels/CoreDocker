using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core.BusinessLogic.Components.Interfaces;
using CoreDocker.Api.Models.Mappers;

namespace CoreDocker.Api.Common
{
    public abstract class ReadOnlyCommonControllerBase<TDal, TModel, TReferenceModel>
    {
        protected IBaseManager<TDal> _projectManager;

        public Task<IEnumerable<TReferenceModel>> Get(string query = null)
        {
            return Task.FromResult(ToReferenceModelList(_projectManager.Query()) as IEnumerable<TReferenceModel>);
        }

        public Task<IEnumerable<TModel>> GetDetail(string query = null)
        {
            return Task.FromResult(ToModelList( _projectManager.Query()) as IEnumerable<TModel>);
        }

        public async Task<TModel> GetById(string id)
        {
            var task = await _projectManager.GetById(id);
            return ToModel(task);
        }

        protected virtual TModel ToModel(TDal arg)
        {
            return MapApi.GetInstance().Map<TDal, TModel>(arg);
        }

        protected virtual IEnumerable<TModel> ToModelList(IEnumerable<TDal> arg)
        {
            return MapApi.GetInstance().Map<IEnumerable<TDal>, IEnumerable<TModel>>(arg);
        }

        protected virtual IEnumerable<TReferenceModel> ToReferenceModelList(IEnumerable<TDal> arg)
        {
            return MapApi.GetInstance().Map<IEnumerable<TDal>, IEnumerable<TReferenceModel>>(arg);
        }
    }
}