using System.Linq;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

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
        public void Name_GiveLongString_ShouldFail()
        {
            // arrange
            Setup();
            // assert
            _validator.ShouldHaveValidationErrorFor(project => project.Name, GetRandom.String(200));
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
    }
}