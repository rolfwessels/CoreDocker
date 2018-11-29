using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Subscriptions;
using CoreDocker.Dal.Models.Users;
using MediatR;

namespace CoreDocker.Core.Components.Users
{
    public class UserRealTimeEventHandler :
        INotificationHandler<UserCreate.Notification>,
        INotificationHandler<UserUpdate.Notification>,
        INotificationHandler<UserRemove.Notification>
    {
        private readonly SubscriptionNotifications _subscription;

        public UserRealTimeEventHandler(SubscriptionNotifications subscription)
        {
            _subscription = subscription;
        }

        #region Implementation of INotificationHandler<in Notification>

        public Task Handle(UserCreate.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification, "UserCreated"));
        }

        #endregion

        #region Implementation of INotificationHandler<in Notification>

        public Task Handle(UserUpdate.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification, "UserUpdated"));
        }

        #endregion

        #region Implementation of INotificationHandler<in Notification>

        public Task Handle(UserRemove.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification, "UserRemoved"));
        }

        #endregion

        private static RealTimeNotificationsMessage BuildMessage(CommandNotificationBase notification, string eventName)
        {
            return new RealTimeNotificationsMessage()
            {
                CorrelationId = notification.CorrelationId,
                Event = eventName,
                Id = notification.Id
            };
        }
    }
}