using System.Collections.Generic;
using System.Linq;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Users;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UserSpecification : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            Name = "User";
            descriptor.Field(d => d.Id).Description("The id of the user.");
            descriptor.Field(d => d.Name).Description("The name of the user.");
            descriptor.Field(d => d.Email).Description("The email of the user.");
            descriptor.Field(d => d.Roles).Description("The roles of the user.");
            descriptor.Field("activities")
                .Type<ListType<StringType>>()
                .Resolver(context => Roles(context.Parent<User>()?.Roles))
                .Description("The activities that this user is authorized for.");
            descriptor.Field(d => d.UpdateDate).Type<DateTimeType>()
                .Description("The date when the user was last updated.");
            descriptor.Field(d => d.CreateDate).Type<NonNullType<DateTimeType>>()
                .Description("The date when the user was created.");
        }

        #region Private Methods

        private static List<string> Roles(List<string> sourceRoles)
        {
            var roles = sourceRoles.Select(RoleManager.GetRole)
                .Where(x => x != null)
                .SelectMany(x => x.Activities)
                .Distinct()
                .Select(x => x.ToString())
                .OrderBy(x => x)
                .ToList();
            return roles;
        }

        #endregion
    }
}