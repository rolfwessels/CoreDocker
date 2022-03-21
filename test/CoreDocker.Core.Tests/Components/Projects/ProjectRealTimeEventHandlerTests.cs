using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Core.Framework.Subscriptions;
using CoreDocker.Core.Tests.Components.Users;
using CoreDocker.Core.Tests.Framework.BaseManagers;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Tests;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Components.Projects
{
    [TestFixture]
    public class ProjectRealTimeEventHandlerTests : BaseManagerTests
    {
        private SubscriptionNotifications _subscriptionNotifications = null!;
        private ProjectRealTimeEventHandler _projectRealTimeEventHandler = null!;

        #region Overrides of BaseManagerTests

        public override void Setup()
        {
            base.Setup();
            _subscriptionNotifications = new SubscriptionNotifications(new Messenger());
            _projectRealTimeEventHandler = new ProjectRealTimeEventHandler(_subscriptionNotifications);
        }

        #endregion

        [Test]
        public void Scan_GivenProjectRealTimeEventHandler_ShouldNotBeMissingAnyNotifications()
        {
            Setup();
            SubscribeHelper.NotificationScanner(typeof(ProjectRealTimeEventHandler));
        }

        [Test]
        public void Handle_GivenProjectCreateNotification_ShouldNotifyOfProjectChange()
        {
            // arrange
            Setup();
            var notification = BuildNotification<ProjectCreate.Notification>();
            // action
            BasicTest(() => _projectRealTimeEventHandler.Handle(notification, CancellationToken.None), notification,
                "ProjectCreated", _subscriptionNotifications);
        }

        [Test]
        public void Handle_GivenProjectUpdateNotification_ShouldNotifyOfProjectChange()
        {
            // arrange
            Setup();
            var notification = BuildNotification<ProjectUpdateName.Notification>();
            // action
            BasicTest(() => _projectRealTimeEventHandler.Handle(notification, CancellationToken.None), notification,
                "ProjectUpdatedName", _subscriptionNotifications);
        }

        [Test]
        public void Handle_GivenProjectRemoveNotification_ShouldNotifyOfProjectChange()
        {
            // arrange
            Setup();
            var notification = BuildNotification<ProjectRemove.Notification>();
            // action
            BasicTest(() => _projectRealTimeEventHandler.Handle(notification, CancellationToken.None), notification,
                "ProjectRemoved", _subscriptionNotifications);
        }


        public void BasicTest(Action action, CommandNotificationBase notification,
            string @event, SubscriptionNotifications subscriptionNotifications)
        {
            var list = new List<RealTimeNotificationsMessage>();
            using (subscriptionNotifications.Register(message => list.Add(message)))
            {
                action();
                // assert
                list.WaitFor(x=>x.Count == 1);
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