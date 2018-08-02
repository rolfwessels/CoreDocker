using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreDocker.Api.Mappers;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Shared.Interfaces.Base;

namespace CoreDocker.Api.WebApi.Controllers
{
    public abstract class BaseCommonController<TDal, TModel, TReferenceModel, TDetailModel> : ReadOnlyCommonControllerBase<TDal, TModel, TReferenceModel>, ICrudController<TModel, TDetailModel>
    {
       
        protected BaseCommonController(IBaseManager<TDal> manager)
        {
            _manager = manager;
        }

     
        public virtual async Task<TModel> Update(string id, TDetailModel model)
        {
            var projectFound = await _manager.GetById(id);
            if (projectFound == null) throw new Exception(string.Format("Could not find model by id '{0}'", id));
            var project = await ToDal(model, projectFound);
            var saveProject = await _manager.Update(project);
            return ToModel(saveProject);
        }


        public virtual async Task<TModel> Insert(TDetailModel model)
        {
            var entity = await ToDal(model);
            var savedProject = await _manager.Insert(entity);
            return ToModel(savedProject);
        }

        public virtual async Task<bool> Delete(string id)
        {
            var deleteProject = await _manager.Delete(id);
            return deleteProject != null;
        }

        protected virtual async Task<TDal> ToDal(TDetailModel model)
        {
            var mappedResult = MapApi.GetInstance().Map<TDetailModel, TDal>(model);
            await AddAdditionalMappings(model, mappedResult);
            return mappedResult;
        }

        protected virtual async Task<TDal> ToDal(TDetailModel model, TDal dal)
        {
            var mappedResult = MapApi.GetInstance().Map(model, dal);
            await AddAdditionalMappings(model, mappedResult);
            return mappedResult;
        }

        protected virtual Task<TDal> AddAdditionalMappings(TDetailModel model, TDal dal)
        {
            return Task.FromResult(dal);
        }
    }
}