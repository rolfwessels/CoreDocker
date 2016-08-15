using Microsoft.AspNetCore.Http;
using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

namespace MainSolutionTemplate.Api.WebApi.Controllers
{
    public static class WebApiHelper
    {
        public static string GetQuery(this HttpRequest request)
        {
            var query = new Uri(request.GetUri().AbsoluteUri);
            return query.Query;
        }
    }
}