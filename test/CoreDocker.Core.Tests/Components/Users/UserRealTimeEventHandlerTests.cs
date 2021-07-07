using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Core.Framework.Subscriptions;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Tests;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Users
{
    [TestFixture]
    public class UserRealTimeEventHandlerTests : BaseManagerTests
    {
        private SubscriptionNotifications _subscriptionNotifications;
        private UserRealTimeEventHandler _userRealTimeEventHandler;

        #region Overrides of BaseManagerTests

        public override void Setup()
        {
            base.Setup();
            _subscriptionNotifications = new SubscriptionNotifications(new Messenger());
            _userRealTimeEventHandler = new UserRealTimeEventHandler(_subscriptionNotifications);
        }

        #endregion

        [Test]
        public void Scan_GivenUserRealTimeEventHandler_ShouldNotBeMissingAnyNotifications()
        {
            Setup();
            SubscribeHelper.NotificationScanner(typeof(UserRealTimeEventHandler));
        }

        [Test]
        public void Handle_GivenUserUpdateNotification_ShouldNotifyOfUserChange()
        {
            // arrange
            Setup();
            var notification = BuildNotification<UserUpdate.Notification>();
            // action
            BasicTest(() => _userRealTimeEventHandler.Handle(notification, CancellationToken.None), notification,
                "UserUpdated");
        }

        [Test]
        public void Handle_GivenUserCreateNotification_ShouldNotifyOfUserChange()
        {
            // arrange
            Setup();
            var notification = BuildNotification<UserCreate.Notification>();
            // action
            BasicTest(() => _userRealTimeEventHandler.Handle(notification, CancellationToken.None), notification,
                "UserCreated");
        }

        [Test]
        public void Handle_GivenUserRemoveNotification_ShouldNotifyOfUserChange()
        {
            // arrange
            Setup();
            var notification = BuildNotification<UserRemove.Notification>();
            // action
            BasicTest(() => _userRealTimeEventHandler.Handle(notification, CancellationToken.None), notification,
                "UserRemoved");
        }


        private void BasicTest(Action action, CommandNotificationBase notification,
            string @event)
        {
            var list = new List<RealTimeNotificationsMessage>();
            using (_subscriptionNotifications.Register(message => list.Add(message)))
            {
                action();
                // assert
                list.WaitFor(messages => messages.Count == 1);
                list.Count.Should().Be(1);
                SubscribeHelper.BasicNotificationValidation(list.First(), notification, @event);
            }
        }

        private static T BuildNotification<T>()
        {
            return Builder<T>.CreateNew().WithValidData().Build();
        }
    }
}