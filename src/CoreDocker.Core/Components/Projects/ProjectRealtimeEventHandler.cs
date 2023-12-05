using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Subscriptions;
using MediatR;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectRealTimeEventHandler : RealTimeEventHandlerBase,
        INotificationHandler<ProjectCreate.Notification>,
        INotificationHandler<ProjectUpdateName.Notification>,
        INotificationHandler<ProjectRemove.Notification>
    {
        private readonly SubscriptionNotifications _subscription;

        public ProjectRealTimeEventHandler(SubscriptionNotifications subscription)
        {
            _subscription = subscription;
        }

        public Task Handle(ProjectCreate.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification));
        }

        public Task Handle(ProjectUpdateName.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification));
        }

        public Task Handle(ProjectRemove.Notification notification, CancellationToken cancellationToken)
        {
            return _subscription.Send(BuildMessage(notification));
        }
    }
}