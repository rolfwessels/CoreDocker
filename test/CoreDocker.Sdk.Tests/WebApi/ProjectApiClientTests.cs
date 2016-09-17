using System.Collections.Generic;
using CoreDocker.Sdk.Tests.Shared;
using NUnit.Framework;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;
using CoreDocker.Sdk.RestApi;
using FizzWare.NBuilder;

namespace CoreDocker.Sdk.Tests.WebApi
{
	[TestFixture]
	[Category("Integration")]
    public class ProjectApiClientTests : CrudComponentTestsBase<ProjectModel, ProjectCreateUpdateModel, ProjectReferenceModel>
	{
		private ProjectApiClient _projectApiClient;

	    #region Setup/Teardown

	    protected override void Setup()
		{
	        var connection = _adminRequestFactory.Value.GetConnection();
	        _projectApiClient = connection.Projects;
            SetRequiredData(_projectApiClient);
		}

	    [TearDown]
		public void TearDown()
		{

		}

	    protected override IList<ProjectCreateUpdateModel> GetExampleData()
	    {
	        var projectDetailModel = Builder<ProjectCreateUpdateModel>.CreateListOfSize(2).All().WithValidModelData().Build();
	        return projectDetailModel;
	    }

	    #endregion
	}

    
}