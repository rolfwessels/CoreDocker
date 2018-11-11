using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Persistence;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Projects
{
    [TestFixture]
    public class ProjectManagerTests : BaseTypedManagerTests<Project>
    {
        private Mock<ILogger<ProjectManager>> _mockLogger;
        private ProjectManager _projectManager;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _mockLogger = new Mock<ILogger<ProjectManager>>();
            _projectManager = new ProjectManager(_baseManagerArguments, _mockLogger.Object);
        }

        #endregion

        protected override IRepository<Project> Repository => _fakeGeneralUnitOfWork.Projects;

        protected override Project SampleObject => Builder<Project>.CreateNew().Build();

        protected override BaseManager<Project> Manager => _projectManager;
    }
}