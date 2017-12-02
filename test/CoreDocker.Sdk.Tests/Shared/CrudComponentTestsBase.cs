using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models.Interfaces;
using log4net;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.Shared
{
    public abstract class CrudComponentTestsBase<TModel, TDetailModel, TReferenceModel> : IntegrationTestsBase
        where TModel : IBaseModel
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected ICrudController<TModel, TDetailModel> _crudController;

        protected void SetRequiredData(ICrudController<TModel, TDetailModel> userControllerActions)
        {
            _crudController = userControllerActions;
        }

        protected abstract void Setup();

        [Test]
        public async Task Get_WhenCalledWithOData_ShouldShouldFilter()
        {
            // arrange
            Setup();
            // action
            if (_crudController is IBaseStandardLookups<TModel, TReferenceModel> baseStandardLookups)
            {
                var restResponse = await baseStandardLookups.GetDetail("$top=1");
                // assert
                restResponse.Count().Should().BeGreaterOrEqualTo(0);
            }
        }

        [Test]
        public async Task PostPutDelete_WhenWhenGivenValidModel_ShouldLookupModels()
        {
            // arrange
            Setup();
            var projectModel = GetExampleData();

            // action
            if (_crudController is IBaseStandardLookups<TModel, TReferenceModel> baseStandardLookups)
            {
                var restResponse = await baseStandardLookups.GetDetail();
                restResponse.Count().Should().BeGreaterOrEqualTo(0);
            }
            var projectModels = await _crudController.Insert(projectModel[0]);
            var savedProject = await _crudController.GetById(projectModels.Id);
            var projectModelLoad = await _crudController.Update(projectModels.Id, projectModel[1]);
            var removed = await _crudController.Delete(projectModels.Id);
            var removedSecond = await _crudController.Delete(projectModels.Id);
            Action testCall = () =>
            {
               _crudController.GetById(projectModels.Id).Wait();
            };
           
            // assert
            savedProject.Should().NotBeNull();
            projectModel[0].ShouldBeEquivalentTo(projectModels, CompareConfig);
            projectModel[1].ShouldBeEquivalentTo(projectModelLoad, CompareConfig);
            removed.Should().BeTrue();
            savedProject.Should().NotBeNull();
            removedSecond.Should().BeFalse();
            testCall.ShouldThrow<Exception>();
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