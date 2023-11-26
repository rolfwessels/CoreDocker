using System;
using System.Reflection;
using System.Threading;
using CoreDocker.Core.Framework.Subscriptions;
using HotChocolate.Subscriptions;
using Serilog;

namespace CoreDocker.Api.GraphQl
{
    public class SubscriptionSubscribe
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly ITopicEventSender _eventSender;
        private int _counter;
        private readonly Lazy<IDisposable> _disposable;

        public SubscriptionSubscribe(SubscriptionNotifications notifications, ITopicEventSender eventSender)
        {
            _eventSender = eventSender;
            _disposable = new Lazy<IDisposable>(() => notifications.Register(SendValue));
        }

        private void SendValue(RealTimeNotificationsMessage message)
        {
            _eventSender.SendAsync(nameof(RealTimeNotificationsMessage), message).AsTask().Wait(10000);
        }

        public void AddSubscription(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _counter);
            _log.Information("Subscription added [{counter}]", _counter);
            if (!_disposable.IsValueCreated)
            {
                _log.Debug("SubscriptionSubscribe:AddSubscription create subscriptions {value}", _disposable.Value);
            }

            cancellationToken.Register(() =>
            {
                _log.Information("Subscription removed [{ct}]", _counter);
                Interlocked.Decrement(ref _counter);
            });
        }
    }
}