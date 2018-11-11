using System.Linq;
using CoreDocker.Api.WebApi.Controllers;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Shared.Models.Shared;
using CoreDocker.Utilities.Tests.Tools;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Common
{
    public abstract class BaseCommonControllerTests<TDal, TModel, TReferenceModel, TDetailModel, TManager>
        where TDal : BaseDalModelWithId
        where TModel : BaseModel
        where TManager : class, IBaseManager<TDal>
    {
        private BaseCommonController<TDal, TModel, TReferenceModel, TDetailModel> _commonController;
        private Mock<TManager> _mockManager;

        protected virtual TDal SampleItem => Builder<TDal>.CreateNew().Build();


        protected virtual void Setup()
        {
            _commonController = GetCommonController();
            _mockManager = GetManager();
        }

        protected abstract Mock<TManager> GetManager();

        protected abstract BaseCommonController<TDal, TModel, TReferenceModel, TDetailModel> GetCommonController();


        [TearDown]
        public virtual void TearDown()
        {
            _mockManager.VerifyAll();
        }


        [Test]
        public void Constructor_WhenCalled_ShouldNotBeNull()
        {
            // arrange
            Setup();
            // assert
            _commonController.Should().NotBeNull();
        }

        [Test]
        public void Get_GivenRequest_ShouldReturnProjectReferenceModels()
        {
            // arrange
            Setup();
            var reference = Builder<TDal>.CreateListOfSize(2).Build().ToList();
            _mockManager.Setup(mc => mc.Query()).Returns(reference.ToList().AsQueryable());
            // action
            var result = _commonController.Get().Result;
            // assert
            result.Count().Should().Be(2);
        }


        [Test]
        public void GetDetail_GivenRequest_ShouldReturnProjectModel()
        {
            // arrange
            Setup();
            var reference = Builder<TDal>.CreateListOfSize(2).Build();
            _mockManager.Setup(mc => mc.Query())
                .Returns(reference.ToList().AsQueryable());
            // action
            var result = _commonController.GetDetail().Result;
            // assert
            result.Count().Should().Be(2);
        }


        [Test]
        public void Get_GivenProjectId_ShouldCallGetProject()
        {
            // arrange
            Setup();
            var dal = SampleItem;
            _mockManager.Setup(mc => mc.GetById(dal.Id)).Returns(dal);
            // action
            var result = _commonController.GetById(dal.Id).Result;
            // assert
            result.Id.Should().Be(dal.Id);
        }


       

        protected virtual void AddAdditionalMappings(TDetailModel model, TDal dal)
        {
        }


    }
}