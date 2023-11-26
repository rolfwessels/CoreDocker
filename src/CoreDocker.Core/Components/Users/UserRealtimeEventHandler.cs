using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Subscriptions;
using MediatR;

namespace CoreDocker.Core.Components.Users
{
    public class UserRealTimeEventHandler : RealTimeEventHandlerBase, INotificationHandler<UserCreate.Notification>,
        INotificationHandler<UserUpdate.Notification>,
        INotificationHandler<UserRemove.Notification>
    {
        private readonly SubscriptionNotifications _subscription;

        public UserRealTimeEventHandler(SubscriptionNotifications subscription)
        {
            _subscription = subscription;
        }

        public Task Handle(UserCreate.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification));
        }

        public Task Handle(UserUpdate.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification));
        }

        public Task Handle(UserRemove.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification));
        }
    }
}