using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Users
{
    [TestFixture]
    public class UserGrantManagerTests : BaseTypedManagerTests<UserGrant>
    {
        private Mock<ILogger<UserGrantLookup>> _mockLogger;
        private UserGrantLookup _userGrantLookup;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _mockLogger = new Mock<ILogger<UserGrantLookup>>();
            _userGrantLookup = new UserGrantLookup(_baseManagerArguments, _mockLogger.Object);
        }

        #endregion

        protected override IRepository<UserGrant> Repository => _fakeGeneralUnitOfWork.UserGrants;

        protected override UserGrant SampleObject => Builder<UserGrant>.CreateNew().Build();

        protected override BaseLookup<UserGrant> Lookup => _userGrantLookup;
    }
}