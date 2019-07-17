using System.Linq;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Users
{
    [TestFixture]
    public class UserManagerTests : BaseTypedManagerTests<User>
    {
        private Mock<ILogger<UserLookup>> _mockLogger;
        private UserLookup _userLookup;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _mockLogger = new Mock<ILogger<UserLookup>>();
            _userLookup = new UserLookup(_baseManagerArguments, _mockLogger.Object);
        }

        #endregion


        [Test]
        public void GetUserByEmail_WhenCalledWithExistingUserWithInvalidEmail_ShouldReturnNull()
        {
            // arrange
            Setup();
            var user = _fakeGeneralUnitOfWork.Users.AddFake().First();
            // action
            var userFound = _userLookup.GetUserByEmail(user.Email + "123").Result;
            // assert
            userFound.Should().BeNull();
        }

        [Test]
        public void GetUserByEmail_WhenCalledWithExistingUserWithInvalidPassword_ShouldReturnUser()
        {
            // arrange
            Setup();
            var user = _fakeGeneralUnitOfWork.Users.AddFake().First();


            // action
            var userFound = _userLookup.GetUserByEmail(user.Email).Result;
            // assert
            userFound.Should().NotBeNull();
        }

        [Test]
        public void GetUserByEmailAndPassword_WhenCalledWithExistingUsernameAndPassword_ShouldReturnTheUser()
        {
            // arrange
            Setup();
            const string password = "sample";
            var user = _fakeGeneralUnitOfWork.Users.AddAFake(x =>
            {
                x.HashedPassword = PasswordHash.CreateHash(password);
            });
            // action
            var userFound = _userLookup.GetUserByEmailAndPassword(user.Email, password).Result;
            // assert
            userFound.Should().NotBeNull();
        }

        [Test]
        public void GetUserByEmailAndPassword_WhenCalledWithExistingUsernameWithInvalidPassword_ShouldReturnNull()
        {
            // arrange
            Setup();
            const string password = "sample";
            var user = _fakeGeneralUnitOfWork.Users.AddAFake(x =>
            {
                x.HashedPassword = PasswordHash.CreateHash(password);
            });
            // action
            var userFound = _userLookup.GetUserByEmailAndPassword(user.Email, password + 123).Result;
            // assert
            userFound.Should().BeNull();
        }

        [Test]
        public void GetUserByEmailAndPassword_WhenCalledWithInvalidUser_ShouldReturnNull()
        {
            // arrange
            Setup();
            var user = _fakeGeneralUnitOfWork.Users.AddFake().First();
            const string password = "sample";
            user.HashedPassword = PasswordHash.CreateHash(password);
            // action
            var userFound = _userLookup.GetUserByEmailAndPassword(user.Email + "123", password).Result;
            // assert
            userFound.Should().BeNull();
        }


        protected override IRepository<User> Repository => _fakeGeneralUnitOfWork.Users;

        protected override User SampleObject
        {
            get { return Builder<User>.CreateNew().With(x => x.Email = GetRandom.Email()).Build(); }
        }

        protected override BaseLookup<User> Lookup => _userLookup;
    }
}