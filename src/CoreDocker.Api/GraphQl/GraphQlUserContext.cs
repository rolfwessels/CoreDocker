using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Utilities.Helpers;
using HotChocolate.Resolvers;

namespace CoreDocker.Api.GraphQl
{
    public static class GraphQlUserContextHelper
    {
        public static Task<User> GetUser(this IResolverContext context, IUserLookup userLookup)
        {
            return (Task<User>) context.ContextData.GetOrAdd("UserTask",
                () => ReadFromClaimsPrinciple(context, userLookup) as object);
        }

        private static Task<User> ReadFromClaimsPrinciple(IResolverContext context, IUserLookup userLookup)
        {
            if (context.ContextData.TryGetValue("ClaimsPrincipal", out var principle))
            {
                var claimsPrincipal = (ClaimsPrincipal) principle;
                var id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                if (!string.IsNullOrEmpty(id))
                {
                    return userLookup.GetById(id);
                }
            }

            return Task.FromResult<User>(null);
        }
    }
}