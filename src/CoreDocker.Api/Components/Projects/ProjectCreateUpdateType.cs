﻿using CoreDocker.Shared.Models.Projects;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectCreateUpdateType : InputObjectType<ProjectCreateUpdateModel>
    {
        protected override void Configure(IInputObjectTypeDescriptor<ProjectCreateUpdateModel> descriptor)
        {
            Name = "ProjectCreateUpdate";
            descriptor.Field(d => d.Name)
                .Type<NonNullType<StringType>>().Description("The name of the project.");
        }
    }
}