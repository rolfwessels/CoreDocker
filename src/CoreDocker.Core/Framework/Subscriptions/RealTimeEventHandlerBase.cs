using CoreDocker.Core.Framework.CommandQuery;

namespace CoreDocker.Core.Framework.Subscriptions
{
    public class RealTimeEventHandlerBase
    {
        protected RealTimeNotificationsMessage BuildMessage(CommandNotificationBase notification)
        {
            return new RealTimeNotificationsMessage
            (
                notification.Id,
                notification.EventName,
                notification.CorrelationId,
                null
            );
        }
    }
}