﻿using System;
using CoreDocker.Shared.Models.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UserCreateUpdateType : InputObjectType<UserCreateUpdateModel>
    {
        protected override void Configure(IInputObjectTypeDescriptor<UserCreateUpdateModel> descriptor)
        {
            Name = "UserCreateUpdate";
            descriptor.Field(d => d.Name).Type<NonNullType<StringType>>().Description("The name of the user.");
            descriptor.Field(d => d.Email).Type<NonNullType<StringType>>().Description("The email of the user.");
            descriptor.Field(d => d.Roles).Type<NonNullType<ListType<StringType>>>().Description("The users roles.");
            descriptor.Field(d => d.Password).Type<StringType>().Description("The password of the user.");
        }
    }
}