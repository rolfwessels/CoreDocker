using System;
using CoreDocker.Shared.Models;
using Flurl.Http;

namespace CoreDocker.Sdk.OAuth
{
    public class RestClientException : Exception
    {
      
        public RestClientException(ErrorMessage message, Exception innerException) : base(message.Message, innerException)
        {
            FullMessage = message;
        }

        public RestClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ErrorMessage FullMessage { get; set; }
        
    }
}