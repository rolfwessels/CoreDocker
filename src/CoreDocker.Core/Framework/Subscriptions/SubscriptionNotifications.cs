using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.Subscriptions
{
    public class SubscriptionNotifications
    {
        private class SubscriptionWithGroup
        {
            public RealTimeNotificationsMessage Message { get; set; }
        }

        private readonly ISubject<SubscriptionWithGroup> _stream = new ReplaySubject<SubscriptionWithGroup>(0);

        #region Implementation of IRealTimeNotificationChannel

        public Task Send(RealTimeNotificationsMessage message)
        {
            if (message != null) _stream.OnNext(new SubscriptionWithGroup() {Message = message});
            return Task.CompletedTask;
        }

        #endregion

        public IObservable<RealTimeNotificationsMessage> Messages()
        {
            return _stream
                .Select(x => x.Message)
                .AsObservable();
        }
    }

}