using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Tests;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Users
{
    [TestFixture]
    public class UserRemoveTests : BaseManagerTests
    {
        private UserRemove.Handler _handler;
        private IRepository<User> _users;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _handler = new UserRemove.Handler(_inMemoryGeneralUnitOfWorkFactory,
                _mockICommander.Object);
            _users = _fakeGeneralUnitOfWork.Users;
        }

        #endregion


        [Test]
        public async Task ProcessCommand_GivenValidRequest_ShouldAddUser()
        {
            // arrange
            Setup();
            var validRequest = GetValidRequest();
            // action
            await _handler.ProcessCommand(validRequest, CancellationToken.None);
            // assert
            var user = await _users.FindOne(x => x.Id == validRequest.Id);
            user.Should().Be(null);
        }

        public UserRemove.Request GetValidRequest()
        {
            var existingUser = _fakeGeneralUnitOfWork.Users.AddAFake();
            var userDeleteUpdateModels = Builder<User>.CreateNew()
                .WithValidData()
                .With(x => x.Id = existingUser.Id)
                .Build()
                .DynamicCastTo<UserRemove.Request>();
            return userDeleteUpdateModels;
        }
    }
}