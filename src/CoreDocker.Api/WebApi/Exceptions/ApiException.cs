using System;
using System.Net;

namespace CoreDocker.Api.WebApi.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            Exception? innerException = null) : base(message,
            innerException)
        {
            HttpStatusCode = statusCode;
        }

        public HttpStatusCode HttpStatusCode { get; set; }
    }
}