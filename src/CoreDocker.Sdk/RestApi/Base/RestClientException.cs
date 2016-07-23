using System;

namespace MainSolutionTemplate.Sdk.OAuth
{
    public class RestClientException : Exception
    {
        public RestClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
    }
}