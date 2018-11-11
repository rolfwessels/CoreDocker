using CoreDocker.Api.AppStartup;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Tests.Managers;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.DataIntegrity
{
    [TestFixture]
    public class DataIntegrityManagerIntegrationTests : BaseManagerTests
    {
        private IProjectManager _projectManager;
        private IUserManager _userManager;

        #region Setup/Teardown

        public override void Setup()
        {
            base.Setup();
            _userManager = IocApi.Instance.Resolve<IUserManager>();
            _projectManager = IocApi.Instance.Resolve<IProjectManager>();
        }

        #endregion

//        
//        [Test]
//        public async Task UpdateAllReferences_GivenObject_ShouldUpdateTheReferences()   
//        {
//            // arrange
//            Setup();
//            var project = Builder<Project>.CreateNew().WithValidData().Build();
//            var user = Builder<User>.CreateNew().WithValidData().Build();
//            user.DefaultProject = project.ToReference();
//
//            await _projectManager.Insert(project);
//            await _userManager.Insert(user);
//
//            // action
//            project.Name = "NewName";
//            await _projectManager.Update(project);
//            var userFound = await _userManager.GetById(user.Id);
//            await _userManager.Delete(user.Id);
//            await _projectManager.Delete(project.Id);
//
//            // assert
//            userFound.DefaultProject.Name.Should().Be(project.Name);
//
//            
//        }
// 
//        [Test]
//        public async Task UpdateAllReferences_GivenObject_ShouldStopRemovalOfObject()
//        {
//            // arrange
//            Setup();
//            var project = Builder<Project>.CreateNew().WithValidData().Build();
//            var user = Builder<User>.CreateNew().WithValidData().Build();
//            user.DefaultProject = project.ToReference();
//
//            await _projectManager.Insert(project);
//            await _userManager.Insert(user);
//
//            // action
//            Exception ex = null;
//            try
//            {
//                _projectManager.Delete(project.Id).Wait();
//            }
//            catch (Exception e)
//            {
//                ex = e;
//            }
//            await _userManager.Delete(user.Id);
//            await _projectManager.Delete(project.Id);
//
//            // assert
//            ex.Dump("ex");
//            ex.ToFirstExceptionOfException().Should().BeOfType<Exception>();
//            ex.ToFirstExceptionOfException().Message.Should().Be("Could not remove Project [Project: Name1]. It is currently referenced in 1 other data object.");
//        }
    }
}