using System.Security.Claims;
using GraphQL.Authorization;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;

namespace CoreDocker.Api.GraphQl
{
    public class GraphQlUserContext : IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }

        public static GraphQlUserContext BuildFromHttpContext(HttpContext ctx)
        {
            var userContext = new GraphQlUserContext
            {
                User = ctx.User
            };
            return userContext;
        }

        public static GraphQlUserContext ReadFromContext(ValidationContext contextUserContext)
        {
            return (GraphQlUserContext) contextUserContext.UserContext;
        }
    }
}