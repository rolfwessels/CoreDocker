using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Framework.Logging
{
    public static class LoggingHelper
    {
        public static void Info(this ILogger logger, string message)
        {
            logger.LogInformation(message);
        }
    }
}