using CoreDocker.Core.Components.Applications;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Persistance;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Managers
{
    [TestFixture]
    public class ApplicationManagerTests : BaseTypedManagerTests<Application>
    {
        private Mock<ILogger<ApplicationManager>> _mockLogger;
        private ApplicationManager _userManager;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _mockLogger = new Mock<ILogger<ApplicationManager>>();

            _userManager = new ApplicationManager(_baseManagerArguments, _mockLogger.Object);
        }

        #endregion

        protected override IRepository<Application> Repository => _fakeGeneralUnitOfWork.Applications;

        protected override Application SampleObject => Builder<Application>.CreateNew().Build();

        protected override BaseManager<Application> Manager => _userManager;
    }
}