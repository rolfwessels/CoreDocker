using System.Linq;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Models.Users;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace CoreDocker.Dal.Tests.Validation
{
    [TestFixture]
    public class UserValidatorTests
    {
        private UserValidator _validator = null!;

        #region Setup/Teardown

        public void Setup()
        {
            _validator = new UserValidator();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion


        [Test]
        public void Email_GiveEmptyEmail_ShouldFail()
        {
            // arrange
            Setup();
            var user = Builder<User>.CreateNew().WithValidData().Build();
            user.Email = "";

            // assert
            _validator.TestValidate(user).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void Email_GiveInvalidEmail_ShouldFail()
        {
            // arrange
            Setup();
            var user = Builder<User>.CreateNew().WithValidData().Build();
            user.Email = "asdf";
            // assert
            _validator.TestValidate(user).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void Email_GiveValidEmail_ShouldNotFail()
        {
            // arrange
            Setup();
            var user = Builder<User>.CreateNew().WithValidData().Build();
            user.Email = "fasdas@.asd.cp,";
            // assert
            _validator.TestValidate(user).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void HashedPassword_GivenEmptyPassword_ShouldFail()
        {
            // arrange
            Setup();
            var user = Builder<User>.CreateNew().WithValidData().Build();
            user.HashedPassword = null;
            // assert
            _validator.TestValidate(user).ShouldHaveValidationErrorFor(x => x.HashedPassword);
        }


        [Test]
        public void Name_GiveNullName_ShouldFail()
        {
            // arrange
            Setup();
            var user = Builder<User>.CreateNew().WithValidData().Build();
            user.Name = null;
            // assert
            _validator.TestValidate(user).ShouldHaveValidationErrorFor(x => x.Name);
        }


        [Test]
        public void Validate_GiveInvalidRole_ShouldFail()
        {
            // arrange
            Setup();
            var user = Builder<User>.CreateNew().WithValidData().Build();
            user.Roles.Clear();
            // action
            var validationResult = _validator.Validate(user);
            // assert
            validationResult.Errors.Select(x => x.ErrorMessage).Should().Contain("'Roles' must not be empty.");
        }

        [Test]
        public void Validate_GiveValidUserData_ShouldNotFail()
        {
            // arrange
            Setup();
            var singleObjectBuilder = Builder<User>.CreateNew();
            var user = singleObjectBuilder.WithValidData().Build();

            // action
            var validationResult = _validator.Validate(user);
            // assert
            validationResult.Errors.Select(x => x.ErrorMessage).StringJoin().Should().BeEmpty();
            validationResult.IsValid.Should().BeTrue();
        }
    }
}