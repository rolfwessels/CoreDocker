using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Api.Models.Mappers;
using CoreDocker.Core.Framework.BaseManagers;

namespace CoreDocker.Api.WebApi.Controllers
{
    public abstract class ReadOnlyCommonControllerBase<TDal, TModel, TReferenceModel>
    {
        protected IBaseManager<TDal> _projectManager;

        public List<TModel> Query(Func<IQueryable<TDal>, IQueryable<TDal>> apply)
        {
            var queryable = _projectManager.Query();
            queryable = apply(queryable);
            return ToModelList(queryable).ToList();
        }
        
        public Task<IEnumerable<TReferenceModel>> Get(string query = null)
        {
            return Task.FromResult(ToReferenceModelList(_projectManager.Query()));
        }

        public Task<IEnumerable<TModel>> GetDetail(string query = null)
        {
            return Task.FromResult(ToModelList( _projectManager.Query()));
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