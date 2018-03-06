using Microsoft.AspNetCore.Http;

namespace CoreDocker.Api.Common
{
    public static class HttpContextAccessorHelper
    {
        public static string GetName(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.User?.Identity?.Name;
        }
    }
}