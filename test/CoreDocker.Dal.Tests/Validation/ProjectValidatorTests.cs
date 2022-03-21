using System.Linq;
using CoreDocker.Dal.Models.Projects;
using Bumbershoot.Utilities.Helpers;
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
        private ProjectValidator _validator = null!;

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
            var project = Builder<Project>.CreateNew().WithValidData().Build();
            project.Name = GetRandom.String(200);
            // assert
            _validator.TestValidate(project).ShouldHaveValidationErrorFor(project => project.Name);
        }


        [Test]
        public void Name_GiveNullName_ShouldFail()
        {
            // arrange
            Setup();
            var project = Builder<Project>.CreateNew().WithValidData().Build();
            project.Name = null!;
            // assert
            _validator.TestValidate(project).ShouldHaveValidationErrorFor(project => project.Name);

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