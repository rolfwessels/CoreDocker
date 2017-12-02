using System.Collections.Generic;
using CoreDocker.Dal.Models;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.WebApi
{
  [TestFixture]
  [Category("Integration")]
  public class
    ProjectApiClientTests : CrudComponentTestsBase<ProjectModel, ProjectCreateUpdateModel, ProjectReferenceModel>
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

    #endregion

    protected override IList<ProjectCreateUpdateModel> GetExampleData()
    {
      return Builder<Project>.CreateListOfSize(2).WithValidData().Build().DynamicCastTo<List<ProjectCreateUpdateModel>>();
    }
  }
}