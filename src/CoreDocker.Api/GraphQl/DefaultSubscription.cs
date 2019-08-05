using CoreDocker.Core.Framework.Subscriptions;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl
{
    public class Subscription
    {
        public RealTimeNotificationsMessage OnDefaultEvent(IEventMessage message)
        {
            return (RealTimeNotificationsMessage)message.Payload;
        }
    }

    public class DefaultSubscription : ObjectType<Subscription>
    {
        private readonly SubscriptionNotifications _pub;

        protected override void Configure(IObjectTypeDescriptor<Subscription> descriptor)
        {
            descriptor.Field(t => t.OnDefaultEvent(default(IEventMessage)))
                .Type<NonNullType<RealTimeNotificationsMessageType>>();

            //            AddField(new EventStreamFieldType
            //            {
            //                Name = "generalEvents",
            //                Arguments = new QueryArguments(),
            //                Type = typeof(RealTimeNotificationsMessageType),
            //                Resolver = new FuncFieldResolver<RealTimeNotificationsMessage>(Resolver),
            //                Subscriber = new EventStreamResolver<RealTimeNotificationsMessage>(context =>
            //                    Subscribe(context, context.GetArgument<string>("channelId")))
            //            });
        }


//
//        private RealTimeNotificationsMessage Resolver(ResolveFieldContext context)
//        {
//            var userNotificationsMessages = (context.Source as RealTimeNotificationsMessage);
//            return userNotificationsMessages;
//        }
//
//        private IObservable<RealTimeNotificationsMessage> Subscribe(ResolveEventStreamContext context, string channelId)
//        {
////            var messageContext = context.UserContext.As<MessageHandlingContext>();
////            var user = messageContext.Get<ClaimsPrincipal>("user");
//            return _pub.Messages();
//        }
    }

    public class RealTimeNotificationsMessageType : ObjectType<RealTimeNotificationsMessage>
    {
        
    }
}