﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class SubscriptionsClientTests : IntegrationTestsBase
    {
        private UserApiClient _userApiClient;

        #region Setup/Teardown

        protected void Setup()
        {
            TestLoggingHelper.EnsureExists();
            _userApiClient = _adminConnection.Value.Users;
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public async Task OnDefaultEvent_GivenInsertUpdateDelete_ShouldBeValid()
        {
            // arrange
            Setup();
            var userCreate = GetExampleData().First();
            var items = new List<CoreDockerClient.RealTimeEvent>();
            var sendSubscribeGeneralEvents = _adminConnection.Value.SendSubscribeGeneralEvents();
            Exception error = null;
            Action<Exception> onError = e => error = e;
            var subscriptions =
                sendSubscribeGeneralEvents.Subscribe((evt) => items.Add(evt.Data.OnDefaultEvent), onError);

            using (subscriptions)
            {
                await Task.Delay(100);//required to allow subscription
                // action
                var insertCommand = await _userApiClient.Create(userCreate);
                var insert = await _userApiClient.ById(insertCommand.Id);
                await _userApiClient.Remove(insert.Id);

                items.WaitFor(x => x.Count == 2, 10000);
                // onError.Should().BeNull();
                items.Should().HaveCount(2);
                error.Should().BeNull();
                items.Select(x=>x.Event).Should().Contain("UserRemoved");
            }

            subscriptions.Should().NotBeNull();
        }


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