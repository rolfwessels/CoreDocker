using System.Threading;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Tests;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Users
{
    [TestFixture]
    public class UserUpdateTests : BaseManagerTests
    {
        private UserUpdate.Handler _handler = null!;
        private IRepository<User> _users = null!;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _handler = new UserUpdate.Handler(_inMemoryGeneralUnitOfWorkFactory, FakeValidator.New<UserValidator>(),
                _mockICommander.Object);
            _users = _fakeGeneralUnitOfWork.Users;
        }

        #endregion

        [Test]
        public void ProcessCommand_GivenInvalidRequest_ShouldThrowException()
        {
            // arrange
            Setup();
            var validRequest = GetValidRequest() with { Name = "" };
            // action
            var testCall = () => { _handler.ProcessCommand(validRequest, CancellationToken.None).Wait(); };
            // assert
            testCall.Should().Throw<ValidationException>()
                .And.Errors.Should().Contain(x =>
                    x.ErrorMessage == "'Name' must be between 1 and 150 characters. You entered 0 characters.");
        }

        [Test]
        public async Task ProcessCommand_GivenPasswordAsNull_ShouldNotSetPassword()
        {
            // arrange
            Setup();
            var validRequest = GetValidRequest() with { Password = null };
            // action
            await _handler.ProcessCommand(validRequest, CancellationToken.None);
            // assert
            var user = await _users.FindOne(x => x.Id == validRequest.Id);
            user!.IsPassword("existingpass").Should().Be(true);
        }

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
            ;
            user.Should().NotBeNull();
        }

        [Test]
        public async Task ProcessCommand_GivenValidRequest_ShouldSetAllProperties()
        {
            // arrange
            Setup();
            var validRequest = GetValidRequest();
            // action
            await _handler.ProcessCommand(validRequest, CancellationToken.None);
            // assert
            var user = await _users.FindOne(x => x.Id == validRequest.Id);
            user.Should().BeEquivalentTo(validRequest, opt => DefaultCommandExcluding(opt)
                .Excluding(x => x.Password));
            user!.HashedPassword!.Length.Should().BeGreaterThan(validRequest.Password!.Length);
        }

        [Test]
        public async Task ProcessCommand_GivenValidRequest_ShouldSetPassword()
        {
            // arrange
            Setup();
            var validRequest = GetValidRequest() with { Password = "test" };
            // action
            await _handler.ProcessCommand(validRequest, CancellationToken.None);
            // assert
            var user = await _users.FindOne(x => x.Id == validRequest.Id);
            user!.IsPassword("test").Should().Be(true);
        }

        public UserUpdate.Request GetValidRequest()
        {
            var existingUser = _fakeGeneralUnitOfWork.Users.AddAFake(x => x.SetPassword("existingpass"));
            var userUpdateUpdateModels = Builder<User>.CreateNew()
                    .WithValidData()
                    .With(x => x.Id = existingUser.Id)
                    .Build()
                    .DynamicCastTo<UserUpdate.Request>() with
                {
                    Password = "tes"
                };
            return userUpdateUpdateModels;
        }
    }
}