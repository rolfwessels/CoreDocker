using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Subscriptions;
using CoreDocker.Utilities.Helpers;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Serilog;



namespace CoreDocker.Api.GraphQl
{
    public class DefaultSubscription
    {
        private readonly SubscriptionSubscribe _subscribe;

        public DefaultSubscription(SubscriptionSubscribe subscribe)
        {
            _subscribe = subscribe;
        }

        [SubscribeAndResolve]
        public async Task<ISourceStream<RealTimeNotificationsMessage>> OnDefaultEvent(
            [Service] ITopicEventReceiver eventReceiver,
            CancellationToken cancellationToken)
        {
            _subscribe.AddSubscription(cancellationToken);
           

            return await eventReceiver.SubscribeAsync<string, RealTimeNotificationsMessage>(
                nameof(RealTimeNotificationsMessage), cancellationToken);
        }

    }

    public class SubscriptionSubscribe
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ITopicEventSender _eventSender;
        private int _counter;


        public SubscriptionSubscribe(SubscriptionNotifications notifications, ITopicEventSender eventSender)
        {
            _eventSender = eventSender;
            var observable = notifications.Messages();
            observable.Subscribe(SendMessageToEventSender);
        }

        private void SendMessageToEventSender(RealTimeNotificationsMessage message)
        {
            _eventSender.SendAsync(nameof(RealTimeNotificationsMessage), message)
                .AsTask()
                .ContinueWithAndLogError(_log.Error);
        }

        public void AddSubscription(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _counter);
            _log.Information($"Subscription added [{_counter}]");
            cancellationToken.Register(() =>
            {
                _log.Information($"Subscription removed [{_counter}]");
                Interlocked.Decrement(ref _counter);
            });
        }
    }

    public class RealTimeNotificationsMessageType : ObjectType<RealTimeNotificationsMessage>
    {
        #region Overrides of ObjectType<RealTimeNotificationsMessage>

        protected override void Configure(IObjectTypeDescriptor<RealTimeNotificationsMessage> descriptor)
        {
            descriptor.Field(x => x.Id).Type<NonNullType<StringType>>();
            descriptor.Field(x => x.CorrelationId).Type<NonNullType<StringType>>();
            descriptor.Field(x => x.Event).Type<NonNullType<StringType>>();
            descriptor.Field(x => x.Exception);
        }

        #endregion
    }
}