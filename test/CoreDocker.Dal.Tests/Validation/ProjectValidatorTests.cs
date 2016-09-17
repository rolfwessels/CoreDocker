using CoreDocker.Utilities.Helpers;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generator;
using FluentAssertions;
using FluentValidation.TestHelper;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Validation;
using NUnit.Framework;
using System.Linq;

namespace CoreDocker.Dal.Tests.Validation
{
    [TestFixture]
    public class ProjectValidatorTests
    {

        private ProjectValidator _validator;

        #region Setup/Teardown

        public void Setup()
        {
            _validator = new ProjectValidator();
        }

        [TearDown]
        public void TearDown()
        {

        }

        #endregion

        [Test]
        public void Validate_GiveValidProjectData_ShouldNotFail()
        {
            // arrange
            Setup();
            var project = Builder<Project>.CreateNew().WithValidData().Build();
            // action
            var validationResult = _validator.Validate(project);
            // assert
            validationResult.Errors.Select(x => x.ErrorMessage).StringJoin().Should().BeEmpty();
            validationResult.IsValid.Should().BeTrue();
        }

         
        [Test]
        public void Name_GiveNullName_ShouldFail()
        {
            // arrange
            Setup();
            // assert
            _validator.ShouldHaveValidationErrorFor(project => project.Name, null as string);
        }

        [Test]
        public void Name_GiveLongString_ShouldFail()
        {
            // arrange
            Setup();
            // assert
            _validator.ShouldHaveValidationErrorFor(project => project.Name, GetRandom.String(200));
        }

        
         
    }
}