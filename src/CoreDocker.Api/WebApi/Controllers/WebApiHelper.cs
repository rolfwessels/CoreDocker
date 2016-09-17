using Microsoft.AspNetCore.Http;
using System;

namespace CoreDocker.Api.WebApi.Controllers
{
    public static class WebApiHelper
    {
        public static string GetQuery(this HttpRequest request)
        {
            return request.Path;
        }
    }
}