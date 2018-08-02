using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CoreDocker.Api.Security;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Auth;
using GraphQL.Builders;
using GraphQL.Types;
using IdentityModel;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlExtensions
    {
        public static readonly string PermissionsKey = "Permissions";

        public static bool RequiresPermissions(this IProvideMetadata type)
        {
            var permissions = type.GetMetadata<IEnumerable<Activity>>(PermissionsKey, new List<Activity>());
            return permissions.Any();
        }

        public static bool CanAccess(this IProvideMetadata type, IEnumerable<Activity> claims)
        {
            var permissions = type.GetMetadata<IEnumerable<Activity>>(PermissionsKey, new List<Activity>());
            return permissions.All(x => claims?.Contains(x) ?? false);
        }

        public static bool HasPermission(this IProvideMetadata type, Activity permission)
        {
            var permissions = type.GetMetadata<IEnumerable<Activity>>(PermissionsKey, new List<Activity>());
            return permissions.Any(x => string.Equals(x, permission));
        }

        public static void RequirePermission(this IProvideMetadata type, Activity permission)
        {
            var permissions = type.GetMetadata<List<Activity>>(PermissionsKey);

            if (permissions == null)
            {
                permissions = new List<Activity>();
                type.Metadata[PermissionsKey] = permissions;
            }

            permissions.Add(permission);
        }

        public static FieldBuilder<TSourceType, TReturnType> RequirePermission<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> builder, Activity permission)
        {
            builder.FieldType.RequirePermission(permission);
            return builder;
        }

        public static bool CanAccess(this IProvideMetadata type, IEnumerable<Claim> claimsPrincipalClaims)
        {
            var principalClaims = claimsPrincipalClaims.Where(x => x.Type == JwtClaimTypes.Role).ToArray();
            var isAdmin = principalClaims.Any(x => x.Value == RoleManager.Admin.Name);
            if (isAdmin)
                return true;
            var permissions = type.GetMetadata<IEnumerable<Activity>>(PermissionsKey, new List<Activity>());
            return permissions.All(p =>
                principalClaims.Any(x => x.Value == UserClaimProvider.ToPolicyName(p)));
        }
    }
}