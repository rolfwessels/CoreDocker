using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using GraphQL.Authorization;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;

namespace CoreDocker.Api.GraphQl
{
    public class GraphQlUserContext : IProvideClaimsPrincipal
    {
        private readonly Lazy<Task<User>> _lazyUser;

        public GraphQlUserContext(IUserManager userManager, ClaimsPrincipal ctxUser)
        {
            User = ctxUser;
            _lazyUser = new Lazy<Task<User>>(() => userManager.GetUserByEmail(User.Identity.Name));
        }

        public ClaimsPrincipal User { get; }
        public Task<User> CurrentUser => _lazyUser.Value;

        public static GraphQlUserContext BuildFromHttpContext(HttpContext ctx,
            IUserManager userManager)
        {

            var userContext = new GraphQlUserContext(userManager, ctx.User);
            return userContext;
        }

        public static GraphQlUserContext ReadFromContext(ValidationContext contextUserContext)
        {
            return  contextUserContext.UserContext as GraphQlUserContext;
        }

    }

    public static class GraphQlUserContextHelper
    {
        public static Task<User> User<T>(ResolveFieldContext<T> context)
        {
            var graphQlUserContext = (GraphQlUserContext)context.UserContext;
            return graphQlUserContext.CurrentUser;
        }
    }
}