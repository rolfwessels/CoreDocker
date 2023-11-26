using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Subscriptions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;


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
}