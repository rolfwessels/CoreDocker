using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreDocker.Dal.Models;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;
using log4net;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.WebApi
{
  [TestFixture]
  [Category("Integration")]
  public class UserApiClientTests : CrudComponentTestsBase<UserModel, UserCreateUpdateModel, UserReferenceModel>
  {
      private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private UserApiClient _userApiClient;

    #region Setup/Teardown

    protected override void Setup()
    {
      var connection = _adminRequestFactory.Value.GetConnection();
      _userApiClient = connection.Users;
      SetRequiredData(_userApiClient);
    }

    [TearDown]
    public void TearDown()
    {
    }

    #endregion

    [Test]
    public void Insert_WhenCalledWithInvalidDuplicateEmail_ShouldThrowException()
    {
      // arrange
      Setup();
      var userDetailModel = Builder<UserCreateUpdateModel>.CreateNew().With(x => x.Name = "should fail").Build();
      // action
      Action testCall = () => { _crudController.Insert(userDetailModel).Wait(); };
      // assert
      testCall.ShouldThrow<Exception>().WithMessage("'Email' is not a valid email address.");
    }

    [Test]
    public void Roles_WhenCalled_ShouldReturnAllRoleInformation()
    {
      // arrange
      Setup();
      // action
      var userModel = _userApiClient.Roles().Result;
      // assert
      userModel.Count.Should().BeGreaterOrEqualTo(2);
      userModel.Select(x => x.Name).Should().Contain("Admin");
    }

    [Test]
    public void WhoAmI_GivenUserData_ShouldReturn()
    {
      // arrange
      Setup();
      // action
      var userModel = _userApiClient.WhoAmI().Result;
      // assert
      userModel.Should().NotBeNull();
      userModel.Email.Should().Be("admin");
    }

    #region Overrides of CrudComponentTestsBase<UserModel,UserCreateUpdateModel>

    protected override IList<UserCreateUpdateModel> GetExampleData()
    {
      return Builder<User>.CreateListOfSize(2).WithValidData().Build().DynamicCastTo<List<UserCreateUpdateModel>>();
    }

    #endregion
  }
}