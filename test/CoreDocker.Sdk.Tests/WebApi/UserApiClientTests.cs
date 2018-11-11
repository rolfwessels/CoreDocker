﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.WebApi
{
    [TestFixture]
    [Category("Integration")]
    public class UserApiClientTests : IntegrationTestsBase
    {
        private UserApiClient _userApiClient;

        #region Setup/Teardown

        protected void Setup()
        {
            _userApiClient = _adminConnection.Value.Users;
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public async Task UserCrud_GivenInsertUpdateDelete_ShouldBeValid()
        {
            // arrange
            Setup();
            var data = GetExampleData();
            var userCreate = data.First();
            var userUpdate = data.Last();
            // action
            var insert = await _userApiClient.Create(userCreate);
            Action testUpdateValidationFail = () =>
            {
                var invalidEmailUser = userCreate.DynamicCastTo<UserCreateUpdateModel>();
                invalidEmailUser.Email = "test@sdfsd";
                _userApiClient.UpdateIt(insert.Id, invalidEmailUser).Wait();
            };
            testUpdateValidationFail.Should().Throw<GraphQlResponseException>().WithMessage("'Email' is not a valid email address.");
            var update = await _userApiClient.UpdateIt(insert.Id, userUpdate);
            var getById = await _userApiClient.ById(insert.Id);
            var allAfterUpdate = await _userApiClient.All();
            var deleteResults = await _userApiClient.Remove(insert.Id);
            var deleteResults1 = await _userApiClient.Remove(insert.Id);

            // assert
            insert.Should().BeEquivalentTo(userCreate, CompareConfig);
            update.Should().BeEquivalentTo(userUpdate, CompareConfig);
            getById.Should().BeEquivalentTo(update,r=>r.Excluding(x=>x.UpdateDate));
            allAfterUpdate.Count.Should().BeGreaterThan(0);
            allAfterUpdate.Should().Contain(x => x.Name == update.Name);
            deleteResults.Should().BeTrue();
            deleteResults1.Should().BeFalse();
        }

        [Test]
        public void Create_GivenInvalidModel_ShouldFail()
        {
            // arrange
            Setup();
            var invalidEmailUser = GetExampleData().First();
            invalidEmailUser.Email = "test@sdfsd";
            // action
            Action testUpdateValidationFail = () => { _userApiClient.Create(invalidEmailUser).Wait(); };
            // assert
            testUpdateValidationFail.Should().Throw<GraphQlResponseException>()
                .WithMessage("'Email' is not a valid email address.");
        }

        [Test]
        public void Create_GivenGuestUser_ShouldFail()
        {
            // arrange
            Setup();
            var invalidEmailUser = GetExampleData().First();
            invalidEmailUser.Email = "test@sdfsd";
            // action
            Action testUpdateValidationFail = () => { _guestConnection.Value.Users.Create(invalidEmailUser).Wait(); };
            // action
            testUpdateValidationFail.Should().Throw<GraphQlResponseException>()
                .WithMessage("You are not authorized to run this query.");
        }

        [Test]
        public void Me_GivenNoUser_ShouldFail()
        {
            // arrange
            Setup();
            var newConnection = NewClientNotAuthorized();
            // action
            Action testUpdateValidationFail = () => { newConnection.Users.Me().Wait(); };
            // action
            testUpdateValidationFail.Should().Throw<GraphQlResponseException>()
                .WithMessage("Authentication required.");
        }


        [Test]
        public async Task Roles_GivenNoUser_ShouldNotFail()
        {
            // arrange
            Setup();
            var newConnection = NewClientNotAuthorized();
            // action
            var roles = await newConnection.Users.Roles();
            // action
            roles.Should().HaveCount(2);
        }


        [Test]
        public async Task GraphQl_RegisterANewUser_ShouldWorkWhenNotLoggedIn()
        {
            // arrange
            Setup();
            var register = GetExampleData().First();
            var newClientNotAuthorized = NewClientNotAuthorized();
            // action
            var insert = await newClientNotAuthorized.Users.Register(register);
            var deleteResults = await _userApiClient.Remove(insert.Id);
            // assert
            deleteResults.Should().BeTrue();
        }


        #region Overrides of CrudComponentTestsBase<UserModel,UserCreateUpdateModel,UserReferenceModel>

        protected EquivalencyAssertionOptions<UserCreateUpdateModel> CompareConfig(
            EquivalencyAssertionOptions<UserCreateUpdateModel> options)
        {
            return options.Excluding(x => x.Password);
        }

        #endregion

        #region Overrides of CrudComponentTestsBase<UserModel,UserCreateUpdateModel>

        protected IList<UserCreateUpdateModel> GetExampleData()
        {
            var userCreateUpdateModels = Builder<User>.CreateListOfSize(2).WithValidData().Build()
                .DynamicCastTo<List<UserCreateUpdateModel>>();
            userCreateUpdateModels.ForEach(x => x.Password = GetRandom.Phrase(20));
            return userCreateUpdateModels;
        }

        #endregion
    }
}