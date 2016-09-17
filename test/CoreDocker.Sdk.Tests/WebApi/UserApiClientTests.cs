using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using CoreDocker.Sdk.Tests.Shared;
using NUnit.Framework;
using log4net;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;
using CoreDocker.Sdk.RestApi;
using FizzWare.NBuilder;

namespace CoreDocker.Sdk.Tests.WebApi
{
	[TestFixture]
	[Category("Integration")]
    public class UserApiClientTests : CrudComponentTestsBase<UserModel, UserCreateUpdateModel, UserReferenceModel>
	{
		private static readonly ILog _log = LogManager.GetLogger<UserApiClientTests>();
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
        public void Insert_WhenCalledWithInvalidDuplicateEmail_ShouldThrowException()
        {
            // arrange
            Setup();
            var userDetailModel = Builder<UserCreateUpdateModel>.CreateNew().With(x=>x.Name = "should fail").Build();
            // action
            Action testCall = () => { _crudController.Insert(userDetailModel).Wait(); };
            // assert
            testCall.ShouldThrow<Exception>().WithMessage("'Email' is not a valid email address.");
        }

        #region Overrides of CrudComponentTestsBase<UserModel,UserCreateUpdateModel>

        protected override IList<UserCreateUpdateModel> GetExampleData()
        {
            return Builder<UserCreateUpdateModel>.CreateListOfSize(2).All().WithValidModelData().Build();
        }

        #endregion

	}

    
}