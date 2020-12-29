using System;
using System.Collections.Generic;
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

// hot chocolate complains that IEventSender is obsolete but the alternative does not exist.
#pragma warning disable 618

namespace CoreDocker.Api.GraphQl
{
    public class DefaultSubscription
    {


        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public DefaultSubscription(SubscriptionSubscribe subscribe)
        {

        }

        [SubscribeAndResolve]
        public async Task<ISourceStream<RealTimeNotificationsMessage>> OnDefaultEvent(
            [Service] ITopicEventReceiver eventReceiver,
            CancellationToken cancellationToken)
        {
            _log.Warning("!!!!!!!!!!!!!!!!  OnDefaultEvent - subscribe");
            cancellationToken.Register(() => _log.Warning("!!!!!!!!!!!!!!!! OnDefaultEvent - unsubscribe"));

            return await eventReceiver.SubscribeAsync<string, RealTimeNotificationsMessage>(
                nameof(RealTimeNotificationsMessage), cancellationToken);
        }

    }

    public class SubscriptionSubscribe
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ITopicEventSender _eventSender;

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