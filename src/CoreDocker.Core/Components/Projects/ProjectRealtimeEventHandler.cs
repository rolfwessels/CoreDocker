using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Subscriptions;
using MediatR;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectRealTimeEventHandler :
        INotificationHandler<ProjectCreate.Notification>,
        INotificationHandler<ProjectUpdate.Notification>,
        INotificationHandler<ProjectRemove.Notification>
    {
        private readonly SubscriptionNotifications _subscription;

        public ProjectRealTimeEventHandler(SubscriptionNotifications subscription)
        {
            _subscription = subscription;
        }

        #region Implementation of INotificationHandler<in Notification>

        public Task Handle(ProjectCreate.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification, "ProjectCreated"));
        }

        #endregion

        #region Implementation of INotificationHandler<in Notification>

        public Task Handle(ProjectUpdate.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification, "ProjectUpdated"));
        }

        #endregion

        #region Implementation of INotificationHandler<in Notification>

        public Task Handle(ProjectRemove.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification, "ProjectRemoved"));
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