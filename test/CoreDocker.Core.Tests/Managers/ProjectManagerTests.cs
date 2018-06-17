using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.BaseManagers;
using FizzWare.NBuilder;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Persistance;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Managers
{
    [TestFixture]
    public class ProjectManagerTests : BaseTypedManagerTests<Project>
    {
        private ProjectManager _projectManager;
        private Mock<ILogger<ProjectManager>> _mockLogger;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _mockLogger = new Mock<ILogger<ProjectManager>>();
            _projectManager = new ProjectManager(_baseManagerArguments, _mockLogger.Object);
        }

        #endregion

        protected override IRepository<Project> Repository
        {
            get { return _fakeGeneralUnitOfWork.Projects; }
        }

        protected override Project SampleObject
        {
            get { return Builder<Project>.CreateNew().Build(); }
        }

        protected override BaseManager<Project> Manager
        {
            get { return _projectManager; }
        }
    }
}