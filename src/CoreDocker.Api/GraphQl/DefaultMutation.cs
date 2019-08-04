using System.Threading.Tasks;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl
{
    public class DefaultMutation : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            Name = "Mutation";
            descriptor.Field("projects").Type<ProjectsMutationSpecification>()
                .Resolver(x => x.Resolver<ProjectsMutation>());
//            descriptor.Field("users").Type<UsersMutationSpecification>()
//                .Resolver(x => x.Resolver<UsersMutationSpecification>());
        }
    }
}
