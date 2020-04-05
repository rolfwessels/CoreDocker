using System;
using System.Reflection;
using CoreDocker.Core.Framework.Subscriptions;
using CoreDocker.Utilities.Helpers;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Serilog;

namespace CoreDocker.Api.GraphQl
{
    

    public class DefaultSubscription : ObjectType<Subscription>
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEventSender _eventSender;

        public DefaultSubscription(SubscriptionNotifications notifications, IEventSender eventSender)
        {
            _eventSender = eventSender;
            var observable = notifications.Messages();
            observable.Subscribe(SendMessageToEventSender);
        }

        private void SendMessageToEventSender(RealTimeNotificationsMessage message)
        {
            _eventSender.SendAsync(new OnReviewMessage(message))
                .AsTask()
                .ContinueWithAndLogError(_log.Error); 
        }

        public class OnReviewMessage : EventMessage
        {
            public OnReviewMessage(RealTimeNotificationsMessage message)
                : base(CreateEventDescription(), message)
            {
            }

            private static EventDescription CreateEventDescription()
            {
                return new EventDescription("onDefaultEvent");
            }
        }

        protected override void Configure(IObjectTypeDescriptor<Subscription> descriptor)
        {
            descriptor.Field(t => t.OnDefaultEvent(default(IEventMessage)))
                .Type<NonNullType<RealTimeNotificationsMessageType>>();
        }

    }

    public class Subscription
    {
        public RealTimeNotificationsMessage OnDefaultEvent(IEventMessage message)
        {
            return (RealTimeNotificationsMessage)message.Payload;
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