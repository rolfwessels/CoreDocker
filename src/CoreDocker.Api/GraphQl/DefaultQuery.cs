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
            descriptor.Field("xas").Type<StringType>().Resolver(x => "");
            descriptor.Field("projects")
                .Type<ProjectsQuerySpecification>()
                .Resolver(x => new object());
//                .Resolver(x=>x.Resolver<ProjectsQuerySpecification>());
//            descriptor.Field("users").Type<UsersQuerySpecification>()
//                .Resolver(x => x.Resolver<UsersQuerySpecification>());
        }
    }
}
