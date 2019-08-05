using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Utilities.Helpers;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Serilog;

namespace CoreDocker.Api.GraphQl
{

    public static class GraphQlUserContextHelper
    {

        public static Task<User> GetUser(this IResolverContext context)
        {
            return (Task<User>) context.ContextData.GetOrAdd("UserTask", () => ReadFromClaimsPrinciple(context) as object);
        }

        private static Task<User> ReadFromClaimsPrinciple(IResolverContext context)
        {
            if (context.ContextData.TryGetValue("ClaimsPrincipal", out var principle))
            {
                var claimsPrincipal = (ClaimsPrincipal) principle;
                var id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                if (!string.IsNullOrEmpty(id))
                {
                    var userLookup = context.Resolver<IUserLookup>();
                    return userLookup.GetById(id);
                }
            }

            return Task.FromResult<User>(null);
        }
    }
}
