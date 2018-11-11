using CoreDocker.Core.Framework.DataIntegrity;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Dal.Persistance;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Framework.BaseManagers
{
    public class BaseManagerArguments
    {
        protected readonly IDataIntegrityManager _dataIntegrityManager;
        protected readonly IGeneralUnitOfWork _generalUnitOfWork;
        protected readonly IMessenger _messenger;
        protected readonly IValidatorFactory _validationFactory;

        public BaseManagerArguments(IGeneralUnitOfWork generalUnitOfWork, IMessenger messenger,
            IValidatorFactory validationFactory)
        {
            _generalUnitOfWork = generalUnitOfWork;
            _messenger = messenger;
            _validationFactory = validationFactory;
            _dataIntegrityManager = new DataIntegrityManager(_generalUnitOfWork, IntegrityOperators.Default);
        }

        public IGeneralUnitOfWork GeneralUnitOfWork => _generalUnitOfWork;

        public IMessenger Messenger => _messenger;

        public IValidatorFactory ValidationFactory => _validationFactory;

        public IDataIntegrityManager DataIntegrityManager => _dataIntegrityManager;
    }
}