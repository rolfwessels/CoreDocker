﻿using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl
{
    public class DefaultQuery : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            Name = "Query";
            descriptor.Field("projects")
                .Type<NonNullType<ProjectsQuerySpecification>>()
                .Resolver(x => new ProjectsQuerySpecification.ProjectQuery());
            descriptor.Field("users")
                .Type<NonNullType<UsersQuerySpecification>>()
                .Resolver(x => new UsersQuerySpecification.UsersQuery());
        }
    }
}