﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Tests;
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
            var userCreate = UserApiClientTests.GetExampleData().First();
            var items = new List<CoreDockerClient.RealTimeEvent>();
            var sendSubscribeGeneralEvents = _adminConnection.Value.SendSubscribeGeneralEvents();
            Exception error = null;
            void OnError(Exception e) => error = e;
            var subscriptions =
                sendSubscribeGeneralEvents.Subscribe(evt => items.Add(evt.Data.OnDefaultEvent), OnError);

            using (subscriptions)
            {
                await Task.Delay(1000);//required to allow subscription
                // action
                var insertCommand = await _userApiClient.Create(userCreate);
                var insert = await _userApiClient.ById(insertCommand.Id);
                await _userApiClient.Remove(insert.Id);

                items.WaitFor(x => x.Count >= 2, 10000);
                items.Select(x => x.Event).Should().Contain("UserRemoved");
                items.Should().HaveCountGreaterOrEqualTo(2);
                error.Should().BeNull();
                
            }

            subscriptions.Should().NotBeNull();
        }
        
    }
}