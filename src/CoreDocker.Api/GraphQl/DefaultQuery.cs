using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl
{
    public class DefaultQuery : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            Name = "Query";
            descriptor.Field("projects").Type<ProjectsQuerySpecification>();
            descriptor.Field("users").Type<UsersQuerySpecification>();
        }
    }
}
