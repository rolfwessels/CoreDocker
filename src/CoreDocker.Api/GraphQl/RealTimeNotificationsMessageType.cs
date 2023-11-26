using CoreDocker.Core.Framework.Subscriptions;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl
{
    public class RealTimeNotificationsMessageType : ObjectType<RealTimeNotificationsMessage>
    {
        protected override void Configure(IObjectTypeDescriptor<RealTimeNotificationsMessage> descriptor)
        {
            descriptor.Field(x => x.Id).Type<NonNullType<StringType>>();
            descriptor.Field(x => x.CorrelationId).Type<NonNullType<StringType>>();
            descriptor.Field(x => x.Event).Type<NonNullType<StringType>>();
            descriptor.Field(x => x.Exception);
        }
    }
}