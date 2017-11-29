using System.Collections.Generic;
using System.Linq;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models.Interfaces;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentAssertions.Equivalency;
using log4net;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.Shared
{
    public abstract class CrudComponentTestsBase<TModel, TDetailModel, TReferenceModel> : IntegrationTestsBase
        where TModel : IBaseModel
    {
        private static readonly ILog _log = LogManager.GetLogger<IntegrationTestsBase>();
        protected ICrudController<TModel, TDetailModel> _crudController;

        protected void SetRequiredData(ICrudController<TModel, TDetailModel> userControllerActions)
        {
            _crudController = userControllerActions;
        }

        protected abstract void Setup();

        [Test]
        public void Get_WhenCalledWithOData_ShouldShouldFilter()
        {
            // arrange
            Setup();
            // action
            if (_crudController is IBaseStandardLookups<TModel, TReferenceModel> baseStandardLookups)
            {
                var restResponse = baseStandardLookups.GetDetail("$top=1").Result;
                // assert
                restResponse.Count().Should().BeGreaterOrEqualTo(0);
            }
        }

        [Test]
        public void PostPutDelete_WhenWhenGivenValidModel_ShouldLookupModels()
        {
            // arrange
            Setup();
            var projectModel = GetExampleData();

            // action
            var projectModels = _crudController.Insert(projectModel[0]).Result;
            var savedProject = _crudController.GetById(projectModels.Id).Result;
            var projectModelLoad = _crudController.Update(projectModels.Id, projectModel[1]).Result;
            var removed = _crudController.Delete(projectModels.Id).Result;
            var removedSecond = _crudController.Delete(projectModels.Id).Result;
            var removedProject = _crudController.GetById(projectModels.Id).Result;

            // assert
            savedProject.Should().NotBeNull();
            removedProject.Should().BeNull();
            projectModel[0].ShouldBeEquivalentTo(projectModels, CompareConfig);
            projectModel[1].ShouldBeEquivalentTo(projectModelLoad, CompareConfig);
            removed.Should().BeTrue();
            savedProject.Should().NotBeNull();
            removedSecond.Should().BeFalse();
        }

        protected virtual EquivalencyAssertionOptions<TDetailModel> CompareConfig(
            EquivalencyAssertionOptions<TDetailModel> options)
        {
            return options.ExcludingMissingMembers();
        }

        protected virtual IList<TDetailModel> GetExampleData()
        {
            return Builder<TDetailModel>.CreateListOfSize(2).All().Build();
        }
    }
}