using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.DataIntegrity;
using CoreDocker.Core.Framework.Logging;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Core.Framework.MessageUtil.Models;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Persistance;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;
using CoreDocker.Utilities.Helpers;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Framework.BaseManagers
{
    public abstract class BaseManager
    {
        protected readonly IGeneralUnitOfWork _generalUnitOfWork;
        protected readonly IMessenger _messenger;
        protected readonly IValidatorFactory _validationFactory;
        protected IDataIntegrityManager _dataIntegrityManager;

        protected BaseManager(BaseManagerArguments baseManagerArguments)
        {
            _generalUnitOfWork = baseManagerArguments.GeneralUnitOfWork;
            _messenger = baseManagerArguments.Messenger;
            _validationFactory = baseManagerArguments.ValidationFactory;
            _dataIntegrityManager = baseManagerArguments.DataIntegrityManager;
        }
    }

    public abstract class BaseManager<T> : BaseManager, IBaseManager<T> where T : BaseDalModelWithId
    {
        private readonly ILogger _log;
        private readonly string _name;

        protected BaseManager(BaseManagerArguments baseManagerArguments, ILogger logger) : base(baseManagerArguments)
        {
            _name = typeof(T).Name;
            _log = logger;
        }

        protected abstract IRepository<T> Repository { get; }

        #region IBaseManager<T> Members

        public Task<List<T>> Get(Expression<Func<T, bool>> filter)
        {
            return Repository.Find(filter);
        }

        public Task<List<T>> Get()
        {
            return Repository.Find(x => true);
        }

        public virtual Task<T> GetById(string id)
        {
            return Repository.FindOne(x => x.Id == id);
        }

        public virtual async Task<T> Delete(string id)
        {
            var project = await GetById(id);
            if (project != null)
            {
                _log.Info($"Remove {_name} [{project}]");
                var count = await _dataIntegrityManager.GetReferenceCount(project);
                if (count > 0)
                    throw new ReferenceException($"Could not remove {typeof(T).Name.UnderScoreAndCamelCaseToHumanReadable()} [{project}]. It is currently referenced in {count} other data object.");
                await Repository.Remove(x => x.Id == id);
                _messenger.Send(new DalUpdateMessage<T>(project, UpdateTypes.Removed));
            }

            return project;
        }

        public IQueryable<T> Query()
        {
            return Repository.Query();
        }

        public async Task<T> Update(T entity)
        {
            DefaultModelNormalize(entity);
            await Validate(entity);
            _log.Info($"Update {_name} [{entity}]");
            var update = await Repository.Update(x => x.Id == entity.Id, entity);
            _dataIntegrityManager.UpdateAllReferences(update).ContinueWithNoWait(LogUpdate);
            _messenger.Send(new DalUpdateMessage<T>(entity, UpdateTypes.Updated));
            return update;
        }


        public async Task<T> Insert(T entity)
        {
            DefaultModelNormalize(entity);
            await Validate(entity);
            _log.Info($"Adding {_name} [{entity}]");
            var insert = await Repository.Add(entity);
            _messenger.Send(new DalUpdateMessage<T>(entity, UpdateTypes.Inserted));
            return insert;
        }

        #endregion

        public virtual async Task<T> Save(T entity)
        {
            var projectFound = await GetById(entity.Id);
            if (projectFound == null) return await Insert(entity);
            return await Update(entity);
        }

        protected virtual void DefaultModelNormalize(T user)
        {
        }

        protected virtual Task Validate(T entity)
        {
            _validationFactory.ValidateAndThrow(entity);
            return Task.FromResult(true);
        }

        #region Private Methods

        private void LogUpdate(Task<long> obj)
        {
            if (obj.Result > 0) _log.Info("{0} referenced items have been updated.");
        }

        #endregion
    }
}