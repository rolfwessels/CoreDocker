using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.BaseManagers;
using FizzWare.NBuilder;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistance;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Managers
{
    [TestFixture]
    public class UserGrantManagerTests : BaseTypedManagerTests<UserGrant>
    {
        private UserGrantManager _userGrantManager;
        private Mock<ILogger<UserGrantManager>> _mockLogger;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _mockLogger = new Mock<ILogger<UserGrantManager>>();
            _userGrantManager = new UserGrantManager(_baseManagerArguments, _mockLogger.Object);
        }

        #endregion

        protected override IRepository<UserGrant> Repository => _fakeGeneralUnitOfWork.UserGrants;

        protected override UserGrant SampleObject => Builder<UserGrant>.CreateNew().Build();

        protected override BaseManager<UserGrant> Manager => _userGrantManager;
    }
}