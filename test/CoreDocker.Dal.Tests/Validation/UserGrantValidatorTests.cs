using CoreDocker.Utilities.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using System.Linq;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder.Generators;

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

        [Test]
        public void User_GiveNull_ShouldFail()
        {
            // arrange
            Setup();
            // assert
            _validator.ShouldHaveValidationErrorFor(userGrant => userGrant.User, null as UserReference);
        }

        [Test]
        public void User_GiveNullEmail_ShouldFail()
        {
            // arrange
            Setup();
            var userReference = Builder<UserReference>.CreateNew()
                .With(x=>x.Email = null)
                .Build();
            // assert
            _validator.ShouldHaveValidationErrorFor(userGrant => userGrant.User, userReference);
        }

        [Test]
        public void User_GiveNullName_ShouldFail()
        {
            // arrange
            Setup();
            var userReference = Builder<UserReference>.CreateNew()
                .With(x => x.Name = null)
                .Build();
            // assert
            _validator.ShouldHaveValidationErrorFor(userGrant => userGrant.User, userReference);
        }


        [Test]
        public void Key_GiveNullKey_ShouldFail()
        {
            // arrange
            Setup();
            // assert
            _validator.ShouldHaveValidationErrorFor(userGrant => userGrant.Key, null as string);
        }

        [Test]
        public void Key_GiveLongString_ShouldFail()
        {
            // arrange
            Setup();
            // assert
            _validator.ShouldHaveValidationErrorFor(userGrant => userGrant.Key, GetRandom.String(200));
        }

        
         
    }
}