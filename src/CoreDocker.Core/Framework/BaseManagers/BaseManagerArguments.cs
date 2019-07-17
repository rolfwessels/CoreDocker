using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Framework.BaseManagers
{
    public class BaseManagerArguments
    {
        protected readonly IGeneralUnitOfWork _generalUnitOfWork;
        protected readonly IMessenger _messenger;
        protected readonly IValidatorFactory _validationFactory;

        public BaseManagerArguments(IGeneralUnitOfWork generalUnitOfWork, IMessenger messenger,
            IValidatorFactory validationFactory)
        {
            _generalUnitOfWork = generalUnitOfWork;
            _messenger = messenger;
            _validationFactory = validationFactory;
        }

        public IGeneralUnitOfWork GeneralUnitOfWork => _generalUnitOfWork;

        public IMessenger Messenger => _messenger;

        public IValidatorFactory ValidationFactory => _validationFactory;
    }
}