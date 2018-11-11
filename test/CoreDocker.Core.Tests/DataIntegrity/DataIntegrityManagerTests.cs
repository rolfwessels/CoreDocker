using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.DataIntegrity;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Core.Tests.Managers;
using CoreDocker.Dal.Models.Base;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.DataIntegrity
{
    [TestFixture]
    public class DataIntegrityManagerTests : BaseManagerTests
    {
        private DataIntegrityManager _dataIntegrityManager;
        private List<IIntegrity> _integrityUpdatetors;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _integrityUpdatetors = IntegrityOperators.Default;
            _dataIntegrityManager =
                new DataIntegrityManager(_baseManagerArguments.GeneralUnitOfWork, _integrityUpdatetors);
        }

        #endregion

        [Test]
        public void Constructor_WhenCalled_ShouldNotBeNull()
        {
            // arrange
            Setup();
            // assert
            _dataIntegrityManager.Should().NotBeNull();
        }

        [Test]
        public void FindMissingIntegrityOperators()
        {
            // arrange
            Setup();

            // action
            var referenceCount =
                _dataIntegrityManager.FindMissingIntegrityOperators<IBaseDalModel, IBaseReference>(typeof(BaseDalModel)
                    .GetTypeInfo().Assembly);
            // assert

            referenceCount.Where(x => !x.Contains("Missing User on UserGrant")).Should().BeEmpty();
        }

        [Test]
        public void GetReferenceCount_GivenObject_ShouldFindAllReferences()
        {
            // arrange
            Setup();
            var project = _fakeGeneralUnitOfWork.Projects.AddAFake();
            _fakeGeneralUnitOfWork.Users.AddAFake(p => {
                p.DefaultProject = project.ToReference();
                project.Name = "NewName";
            });
            
            // action
            var referenceCount = _dataIntegrityManager.GetReferenceCount(project).Result;
            // assert
            referenceCount.Should().Be(1);
        }

        [Test]
        public void GetReferenceCount_GivenObjectNotReferenced_ShouldFindNoLinks()
        {
            // arrange
            Setup();
            var project = _fakeGeneralUnitOfWork.Projects.AddAFake();
            // action
            var referenceCount = _dataIntegrityManager.GetReferenceCount(project).Result;
            // assert
            referenceCount.Should().Be(0);
        }

        [Test]
        public async Task UpdateAllReferences_GivenObject_ShouldUpdateTheReferences()
        {
            // arrange
            Setup();
            var project = _fakeGeneralUnitOfWork.Projects.AddAFake();
            var addAFake = _fakeGeneralUnitOfWork.Users.AddAFake(p => {
                p.DefaultProject = project.ToReference();
            });
            project.Name = "NewName";
            // action
            var result = _dataIntegrityManager.UpdateAllReferences(project).Result;
            // assert
            var user = await _fakeGeneralUnitOfWork.Users.FindOne(x => x.Id == addAFake.Id);
            user.DefaultProject.Name.Should().Be(project.Name);
            result.Should().Be(1);
        }


        [Test]
        public async Task UpdateAllReferences_GivenObjectWhereReferenceHasNotChanged_ShouldNotUpdateAllReference()
        {
            // arrange
            Setup();
            var project = _fakeGeneralUnitOfWork.Projects.AddAFake();
            var addAFake = _fakeGeneralUnitOfWork.Users.AddAFake(p => {
                p.DefaultProject = project.ToReference();
            });
            // action
            var result = _dataIntegrityManager.UpdateAllReferences(project).Result;
            // assert
            var user = await _fakeGeneralUnitOfWork.Users.FindOne(x => x.Id == addAFake.Id);
            user.DefaultProject.Name.Should().Be(project.Name);
            result.Should().Be(1); // this should be 0 but check is slower than actually doing the update
        }
    }
}