using System;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Dal.InMemoryCollections;
using CoreDocker.Dal.Persistence;
using FluentAssertions.Equivalency;
using FluentValidation;
using Moq;
using NUnit.Framework;
using IValidatorFactory = CoreDocker.Dal.Validation.IValidatorFactory;
using ValidatorFactoryBase = CoreDocker.Dal.Validation.ValidatorFactoryBase;

namespace CoreDocker.Core.Tests.Framework.BaseManagers
{
    [TestFixture]
    public class BaseManagerTests
    {
        protected IGeneralUnitOfWork _fakeGeneralUnitOfWork = null!;
        public InMemoryGeneralUnitOfWorkFactory _inMemoryGeneralUnitOfWorkFactory = null!;
        public Mock<ICommander> _mockICommander = null!;
        protected Mock<IMessenger> _mockIMessenger = null!;
        protected Mock<IValidatorFactory> _mockIValidatorFactory = null!;

        #region Setup/Teardown

        public virtual void Setup()
        {
            _mockIMessenger = new Mock<IMessenger>();
            _mockIValidatorFactory = new Mock<IValidatorFactory>();
            _inMemoryGeneralUnitOfWorkFactory = new InMemoryGeneralUnitOfWorkFactory();
            _fakeGeneralUnitOfWork = _inMemoryGeneralUnitOfWorkFactory.GetConnection();
            _mockICommander = new Mock<ICommander>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _mockIValidatorFactory.VerifyAll();
            _mockIMessenger.VerifyAll();
            _mockICommander.VerifyAll();
        }

        #endregion

        protected static EquivalencyAssertionOptions<T> DefaultCommandExcluding<T>(
            EquivalencyAssertionOptions<T> opt) where T : CommandRequestBase
        {
            return opt
                .Excluding(x => x.CreatedAt)
                .Excluding(x => x.CorrelationId);
        }

        internal class FakeValidator : ValidatorFactoryBase
        {
            private readonly object _createInstance;

            public FakeValidator(object createInstance)
            {
                _createInstance = createInstance;
            }

            protected override void TryResolve<T>(out IValidator<T> output)
            {
                output = (IValidator<T>)_createInstance;
            }

            public static IValidatorFactory New<T>()
            {
                return new FakeValidator(Activator.CreateInstance(typeof(T))!);
            }
        }
    }
}