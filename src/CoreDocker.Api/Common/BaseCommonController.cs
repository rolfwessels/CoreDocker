using System;
using System.Threading.Tasks;
using AutoMapper;
using CoreDocker.Api.Models.Mappers;
using CoreDocker.Core.BusinessLogic.Components.Interfaces;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models;

namespace CoreDocker.Api.Common
{
    public abstract class BaseCommonController<TDal, TModel, TReferenceModel, TDetailModel> : ReadOnlyCommonControllerBase<TDal, TModel, TReferenceModel>, ICrudController<TModel, TDetailModel>
    {
       
        protected BaseCommonController(IBaseManager<TDal> projectManager)
        {
            _projectManager = projectManager;
            MapApi.Initialize();
        }


        public async Task<TModel> Update(string id, TDetailModel model)
        {
            var projectFound = await _projectManager.GetById(id);
            if (projectFound == null) throw new Exception(string.Format("Could not find model by id '{0}'", id));
            var project = await ToDal(model, projectFound);
            var saveProject = await _projectManager.Update(project);
            return ToModel(saveProject);
        }


        public async Task<TModel> Insert(TDetailModel model)
        {
            var entity = await ToDal(model);
            var savedProject = await _projectManager.Insert(entity);
            return ToModel(savedProject);
        }

        public async Task<bool> Delete(string id)
        {
            var deleteProject = await _projectManager.Delete(id);
            return deleteProject != null;
        }

        protected virtual async Task<TDal> ToDal(TDetailModel model)
        {
            var mappedResult = Mapper.Map<TDetailModel, TDal>(model);
            await AddAdditionalMappings(model, mappedResult);
            return mappedResult;
        }

        protected virtual async Task<TDal> ToDal(TDetailModel model, TDal dal)
        {
            var mappedResult = Mapper.Map(model, dal);
            await AddAdditionalMappings(model, mappedResult);
            return mappedResult;
        }

        protected virtual Task<TDal> AddAdditionalMappings(TDetailModel model, TDal dal)
        {
            return Task.FromResult(dal);
        }
    }
}