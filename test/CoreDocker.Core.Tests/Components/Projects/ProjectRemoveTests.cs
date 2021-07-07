using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Persistence;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Projects
{
    [TestFixture]
    public class ProjectRemoveTests : BaseManagerTests
    {
        private ProjectRemove.Handler _handler;
        private IRepository<Project> _projects;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _handler = new ProjectRemove.Handler(_inMemoryGeneralUnitOfWorkFactory,
                _mockICommander.Object);
            _projects = _fakeGeneralUnitOfWork.Projects;
        }

        #endregion


        [Test]
        public async Task ProcessCommand_GivenValidRequest_ShouldAddProject()
        {
            // arrange
            Setup();
            var validRequest = GetValidRequest();
            // action
            await _handler.ProcessCommand(validRequest, CancellationToken.None);
            // assert
            var project = await _projects.FindOne(x => x.Id == validRequest.Id);
            project.Should().Be(null);
        }

        public ProjectRemove.Request GetValidRequest()
        {
            var existingProject = _fakeGeneralUnitOfWork.Projects.AddAFake();
            var projectDeleteUpdateModels = Builder<Project>.CreateNew()
                .WithValidData()
                .With(x => x.Id = existingProject.Id)
                .Build()
                .DynamicCastTo<ProjectRemove.Request>();
            return projectDeleteUpdateModels;
        }
    }
}