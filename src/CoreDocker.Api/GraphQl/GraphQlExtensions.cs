using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Auth;
using HotChocolate.Types;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlExtensions
    {
        public static void RequirePermission(this IObjectFieldDescriptor type, Activity permission)
        {
            type.Authorize(RoleManager.GetRoles(permission));
        }

        public static void RequireAuthorization(this IObjectFieldDescriptor type)
        {
            type.Authorize();
        }
    }
}