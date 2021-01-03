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
        private Mock<ILogger<ProjectLookup>> _mockLogger;
        private ProjectLookup _projectLookup;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _mockLogger = new Mock<ILogger<ProjectLookup>>();
            _projectLookup = new ProjectLookup(_fakeGeneralUnitOfWork.Projects);
        }

        #endregion

        protected override IRepository<Project> Repository => _fakeGeneralUnitOfWork.Projects;

        protected override Project SampleObject => Builder<Project>.CreateNew().Build();

        protected override BaseLookup<Project> Lookup => _projectLookup;
    }
}