using GraphQL;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl
{
    public class DefaultSchema : Schema
    {
        public DefaultSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<DefaultQuery>();
            Mutation = resolver.Resolve<DefaultMutation>();
            Subscription = resolver.Resolve<DefaultSubscription>();
        }
    }
}