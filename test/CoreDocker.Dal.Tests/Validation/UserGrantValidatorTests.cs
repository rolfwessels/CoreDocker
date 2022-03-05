using System.Linq;
using CoreDocker.Dal.Models.Users;
using Bumbershoot.Utilities.Helpers;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace CoreDocker.Dal.Tests.Validation
{
    [TestFixture]
    public class UserGrantValidatorTests
    {
        private UserGrantValidator _validator;

        #region Setup/Teardown

        public void Setup()
        {
            _validator = new UserGrantValidator();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public void Key_GiveLongString_ShouldFail()
        {
            // arrange
            Setup();
            var userGrant = Builder<UserGrant>.CreateNew().WithValidData().Build();
            userGrant.Key = GetRandom.String(200);
            // assert
            _validator.TestValidate(userGrant).ShouldHaveValidationErrorFor(grant => grant.Key);
        }


        [Test]
        public void Key_GiveNullKey_ShouldFail()
        {
            // arrange
            Setup();
            var userGrant = Builder<UserGrant>.CreateNew().WithValidData().Build();
            userGrant.Key = null;
            // assert
            _validator.TestValidate(userGrant).ShouldHaveValidationErrorFor(grant => grant.Key);
        }

        [Test]
        public void User_GiveNull_ShouldFail()
        {
            // arrange
            Setup();
            var userGrant = Builder<UserGrant>.CreateNew().WithValidData().Build();
            userGrant.User = null;
            // assert
            _validator.TestValidate(userGrant).ShouldHaveValidationErrorFor(grant => grant.User);
        }

        [Test]
        public void User_GiveNullEmail_ShouldFail()
        {
            // arrange
            Setup();
            var userReference = Builder<UserReference>.CreateNew()
                .With(x => x.Email = null)
                .Build();
            var userGrant = Builder<UserGrant>.CreateNew().WithValidData().Build();
            userGrant.User = userReference;
            // assert
            _validator.TestValidate(userGrant).ShouldHaveValidationErrorFor(grant => grant.User);
        }

        [Test]
        public void User_GiveNullName_ShouldFail()
        {
            // arrange
            Setup();
            var userReference = Builder<UserReference>.CreateNew()
                .With(x => x.Name = null)
                .Build();
            var userGrant = Builder<UserGrant>.CreateNew().WithValidData().Build();
            userGrant.User = userReference;
            // assert
            _validator.TestValidate(userGrant).ShouldHaveValidationErrorFor(grant => grant.User);
        }

        [Test]
        public void Validate_GiveValidUserGrantData_ShouldNotFail()
        {
            // arrange
            Setup();
            var userGrant = Builder<UserGrant>.CreateNew().WithValidData().Build();
            // action
            var validationResult = _validator.Validate(userGrant);
            // assert
            validationResult.Errors.Select(x => x.ErrorMessage).StringJoin().Should().BeEmpty();
            validationResult.IsValid.Should().BeTrue();
        }
    }
}