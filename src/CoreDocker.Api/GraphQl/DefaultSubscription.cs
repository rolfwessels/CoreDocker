using System;
using System.Security.Claims;
using CoreDocker.Core.Framework.Subscriptions;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Subscription;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl
{
    public class DefaultSubscription : ObjectGraphType<object>
    {
        private readonly SubscriptionNotifications _pub;

        public DefaultSubscription(SubscriptionNotifications pub)
        {
            _pub = pub;
            AddField(new EventStreamFieldType
            {
                Name = "generalEvents",
                Arguments = new QueryArguments(),
                Type = typeof(RealTimeNotificationsMessageType),
                Resolver = new FuncFieldResolver<RealTimeNotificationsMessage>(Resolver),
                Subscriber = new EventStreamResolver<RealTimeNotificationsMessage>(context => Subscribe(context, context.GetArgument<string>("channelId")))
            });
        }

        private RealTimeNotificationsMessage Resolver(ResolveFieldContext context)
        {
            var userNotificationsMessages = (context.Source as RealTimeNotificationsMessage);
            return userNotificationsMessages;
        }

        private IObservable<RealTimeNotificationsMessage> Subscribe(ResolveEventStreamContext context, string channelId)
        {
            var messageContext = context.UserContext.As<MessageHandlingContext>();
            var user = messageContext.Get<ClaimsPrincipal>("user");
            return _pub.Messages();
        }
    }

    public class RealTimeNotificationsMessageType : ObjectGraphType<RealTimeNotificationsMessage>
    {
        public RealTimeNotificationsMessageType()
        {
            Field(d => d.Id).Description("The id of the item to be created if created.");
            Field(d => d.Event).Description("The name notification.");
            Field(d => d.CorrelationId).Description("The correlation Id for given command.");
        }
    }
}