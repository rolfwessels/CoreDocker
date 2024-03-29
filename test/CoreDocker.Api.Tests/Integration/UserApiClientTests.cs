﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Tests;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models.Users;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class UserApiClientTests : IntegrationTestsBase
    {
        private UserApiClient _userApiClient = null!;

        #region Setup/Teardown

        protected void Setup()
        {
            _userApiClient = AdminClient().Users;
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public void Create_GivenGuestUser_ShouldFail()
        {
            // arrange
            Setup();
            var invalidEmailUser = GetExampleData().First() with { Email = "test@sdfsd" };
            // action
            var testUpdateValidationFail = () => { GuestClient().Users.Create(invalidEmailUser).Wait(); };
            // action
            testUpdateValidationFail.Should().Throw<Exception>()
                .WithMessage("The current user is not authorized to access this resource.");
        }


        [Test]
        public void Create_GivenInvalidModel_ShouldFail()
        {
            // arrange
            Setup();
            var invalidEmailUser = GetExampleData().First() with { Email = "test#.com" };
            // action
            var testUpdateValidationFail = () => { _userApiClient.Create(invalidEmailUser).Wait(); };
            // assert
            testUpdateValidationFail.Should().Throw<Exception>()
                .WithMessage("'Email' is not a valid email address.");
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
            deleteResults.Id.Should().NotBeEmpty();
        }

        [Test]
        public async Task Me_GivenAdminUser_ShouldContainActivities()
        {
            // arrange
            Setup();
            // action
            var userModel = await AdminClient().Users.Me();
            // action
            userModel.Activities.Should().Contain("ReadUsers");
        }

        [Test]
        public async Task Me_GivenAdminUser_ShouldHaveImage()
        {
            // arrange
            Setup();
            // action
            var userModel = await AdminClient().Users.Me();
            // action
            userModel.Image.Should().StartWith("https://www.gravatar.com/avatar");
        }

        [Test]
        public async Task Me_GivenAdminUser_ShouldNotFail()
        {
            // arrange
            Setup();

            // action
            var userModel = await AdminClient().Users.Me();
            // action
            userModel.Email.Should().Contain("@");
        }

        [Test]
        public void Me_GivenNoUser_ShouldFail()
        {
            // arrange
            Setup();
            var newConnection = NewClientNotAuthorized();
            // action
            var testUpdateValidationFail = () => { newConnection.Users.Me().Wait(); };
            // action
            testUpdateValidationFail.Should().Throw<Exception>()
                .WithMessage("The current user is not authorized to access this resource.");
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
        public async Task UserCrud_GivenInsertUpdateDelete_ShouldBeValid()
        {
            // arrange
            Setup();
            var data = GetExampleData();
            var userCreate = data.First();
            var userUpdate = data.Last();

            // action
            var insertCommand = await _userApiClient.Create(userCreate);
            var insert = await _userApiClient.ById(insertCommand.Id);
            var updateCommand = await _userApiClient.Update(insert.Id, userUpdate);
            var update = await _userApiClient.ById(insertCommand.Id);
            var getById = await _userApiClient.ById(insert.Id);
            var allAfterUpdate = await _userApiClient.List();
            var paged = await _userApiClient.Paged(2);
            var firstDelete = await _userApiClient.Remove(insert.Id);

            // assert
            insert.Should().BeEquivalentTo(userCreate, CompareConfig);
            update.Should().BeEquivalentTo(userUpdate, CompareConfig);
            getById.Should().BeEquivalentTo(update, r => r.Excluding(x => x.UpdateDate));
            allAfterUpdate.Count.Should().BeGreaterThan(0);
            allAfterUpdate.Should().Contain(x => x.Name == update.Name);
            paged.Count.Should().BeGreaterOrEqualTo(paged.Items.Count);
            paged.Items.Count.Should().BeLessOrEqualTo(2);
        }

        protected EquivalencyAssertionOptions<UserCreateUpdateModel> CompareConfig(
            EquivalencyAssertionOptions<UserCreateUpdateModel> options)
        {
            return options.Excluding(x => x.Password);
        }

        public static IList<UserCreateUpdateModel> GetExampleData()
        {
            var userCreateUpdateModels = Builder<User>.CreateListOfSize(2).WithValidData().Build()
                .DynamicCastTo<List<UserCreateUpdateModel>>();
            return userCreateUpdateModels.Select(x => x with { Password = GetRandom.Phrase(20) }).ToList();
        }
    }
}