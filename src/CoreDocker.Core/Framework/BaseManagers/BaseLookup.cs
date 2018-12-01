using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Logging;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Core.Framework.MessageUtil.Models;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Framework.BaseManagers
{
    public abstract class BaseLookup
    {
        protected readonly IGeneralUnitOfWork _generalUnitOfWork;
        protected readonly IMessenger _messenger;
        protected readonly IValidatorFactory _validationFactory;

        protected BaseLookup(BaseManagerArguments baseManagerArguments)
        {
            _generalUnitOfWork = baseManagerArguments.GeneralUnitOfWork;
            _messenger = baseManagerArguments.Messenger;
            _validationFactory = baseManagerArguments.ValidationFactory;
        }
    }

    public abstract class BaseLookup<T> : BaseLookup, IBaseLookup<T> where T : BaseDalModelWithId
    {
        private readonly ILogger _log;

        protected BaseLookup(BaseManagerArguments baseManagerArguments, ILogger logger) : base(baseManagerArguments)
        {
            _log = logger;
        }

        protected abstract IRepository<T> Repository { get; }


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

        public Task<IQueryable<T>> Query(Func<IQueryable<T>, IQueryable<T>> query)
        {
            return Task.Run(()=> query(Repository.Query()));
        }

        public IQueryable<T> Query()
        {
            return Repository.Query();
        }
    }
}