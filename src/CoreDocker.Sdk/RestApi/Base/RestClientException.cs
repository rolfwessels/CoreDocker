using System;

namespace CoreDocker.Sdk.OAuth
{
    public class RestClientException : Exception
    {
        public RestClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
    }
}