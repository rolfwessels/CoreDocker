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
using CoreDocker.Utilities.Tests.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using CoreDocker.Utilities.Tests.Tools;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
            
            var subscriptions = await _adminConnection.Value.SendSubscribeGeneralEventsAsync((evt, _) => items.Add(evt));

            using (subscriptions)
            {
                await Task.Delay(100);
//                // action
                var insertCommand = await _userApiClient.Create(userCreate);
                var insert = await _userApiClient.ById(insertCommand.Id);
                await _userApiClient.Remove(insert.Id);
//
//                // assert
                var expected = 2;
                items.WaitFor(x=>x.Count == expected ,1000);
                if (DateTime.Now > new DateTime(2019, 12, 01)) 
                // The graphql client not supported waiting for new release https://github.com/graphql-dotnet/graphql-client/issues/108
                {
                    
                    items.Should().HaveCount(expected);

                    items.Last().Event.Should().Be("UserRemoved");
                }
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