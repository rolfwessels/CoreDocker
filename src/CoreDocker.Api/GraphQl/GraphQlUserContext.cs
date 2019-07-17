using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using CoreDocker.Utilities.Helpers;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;
using GraphQL.Validation;
using log4net;
using Microsoft.AspNetCore.Http;

namespace CoreDocker.Api.GraphQl
{
    public class GraphQlUserContext : IProvideClaimsPrincipal
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Lazy<Task<User>> _lazyUser;

        public GraphQlUserContext(IUserLookup userLookup, ClaimsPrincipal ctxUser)
        {
            User = ctxUser;
            _lazyUser = new Lazy<Task<User>>(() => userLookup.GetUserByEmail(User.Identity.Name));
        }

        public ClaimsPrincipal User { get; }
        public Task<User> CurrentUser => _lazyUser.Value;

        public static IDictionary<string, object> BuildFromHttpContext(HttpContext ctx,
            IUserLookup userLookup)
        {
            var userContext = new GraphQlUserContext(userLookup, ctx.User);
            return new Dictionary<string, object>()
            {
                {"userContext", userContext},
                {"user", userContext.User}
            };
        }

        public static GraphQlUserContext ReadFromContext(ValidationContext contextUserContext)
        {
            var userContext = (Dictionary<string, object>) contextUserContext.UserContext;
            if (userContext.ContainsKey("userContext")) return userContext["userContext"] as GraphQlUserContext;

            return null;
        }
    }

    public static class GraphQlUserContextHelper
    {
        public static Task<User> User<T>(ResolveFieldContext<T> context)
        {
            var userContext = (Dictionary<string, object>) context.UserContext;
            var graphQlUserContext = (GraphQlUserContext) userContext["userContext"];
            return graphQlUserContext.CurrentUser;
        }
    }
}