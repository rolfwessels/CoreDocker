using System.Linq;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.WebApi.Controllers;
using FizzWare.NBuilder;
using FluentAssertions;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Tests.Tools;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Common
{
    [TestFixture]
    public class UserCommonControllerTests:BaseCommonControllerTests<User, UserModel, UserReferenceModel, UserCreateUpdateModel, IUserManager>
    {
        private Mock<IUserManager> _mockIUserManager;
        private UserCommonController _projectCommonController;
        private Mock<IRoleManager> _mockIRoleManager;
        private Mock<IHttpContextAccessor> _mockIHttpContextAccessor;

        #region Overrides of BaseCommonControllerTests

        protected override void Setup()
        {
            _mockIUserManager = new Mock<IUserManager>(MockBehavior.Strict);
            _mockIRoleManager = new Mock<IRoleManager>(MockBehavior.Strict);
            _mockIHttpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            
            _projectCommonController = new UserCommonController(_mockIUserManager.Object, _mockIRoleManager.Object, _mockIHttpContextAccessor.Object);
            
            base.Setup();
        }

        protected override Mock<IUserManager> GetManager()
        {
            return _mockIUserManager;
        }

        protected override BaseCommonController<User, UserModel, UserReferenceModel, UserCreateUpdateModel> GetCommonController()
        {
            return _projectCommonController;
        }

        #region Overrides of BaseCommonControllerTests<User,UserModel,UserReferenceModel,UserCreateUpdateModel,IUserManager>

        public override void TearDown()
        {
            base.TearDown();
            _mockIUserManager.VerifyAll();
            _mockIRoleManager.VerifyAll();
        }

        #endregion

        #endregion
        

        [Test]
        public void Register_GivenRegisterModel_ShouldAddUser()
        {
            // arrange
            Setup();
            var registerModel = Builder<RegisterModel>.CreateNew().Build();
            var user = Builder<User>.CreateNew().Build();
            _mockIUserManager.Setup(mc => mc.Save(It.Is<User>(x => x.Name == registerModel.Name && x.Roles.Any(r => r == RoleManager.Guest.Name)), registerModel.Password)).Returns(user);
            // action
            var result = _projectCommonController.Register(registerModel).Result;
            // assert
            result.Name.Should().Be(registerModel.Name);
            
        }

        [Test]
        public void ForgotPassword_GivenEmail_ShouldSendAnEmail()
        {
            // arrange
            Setup();
            var user = Builder<User>.CreateNew().Build();
            // action
            var result = _projectCommonController.ForgotPassword(user.Email).Result;
            // assert
            result.Should().BeTrue();

        }


         
    }

}