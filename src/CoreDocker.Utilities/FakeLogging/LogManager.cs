using System;
using Microsoft.Extensions.Logging;

namespace log4net
{
    public class LogManager
    {
        private static ILoggerFactory _logger;

        public static ILog GetLogger<T>()
        {
            if (_logger == null) return new Nothing();
            return new LogAdaptor(_logger.CreateLogger<T>());
        }

        public static void SetLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory;
        }

        #region Nested type: LogAdaptor

        private class LogAdaptor : ILog
        {
            private readonly ILogger _assignedLogger;

            public LogAdaptor(ILogger assignedLogger)
            {
                _assignedLogger = assignedLogger;
            }

            public ILogger Logger => _assignedLogger;

            public void Error(string message)
            {
                _assignedLogger.LogError(message);
            }

            public void Error(string message, Exception exception)
            {
                _assignedLogger.LogError(message, exception);
            }

            public void Warn(string message, Exception exception)
            {
                _assignedLogger.LogWarning(message, exception);
            }

            public void Warn(string message)
            {
                _assignedLogger.LogWarning(message);
            }

            public void Info(string message)
            {
                _assignedLogger.LogInformation(message);
            }

            public void Debug(string message)
            {
                _assignedLogger.LogDebug(message);
            }
        }

        #endregion

        #region Nested type: Nothing

        internal class Nothing : ILog
        {
            public ILogger Logger
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            #region ILog Members

            public void Debug(string message)
            {
            }

            public void Error(string message)
            {
            }

            public void Error(string message, Exception exception)
            {
            }

            public void Info(string message)
            {
            }

            public void Warn(string message)
            {
            }

            public void Warn(string message, Exception exception)
            {
            }

            #endregion
        }

        #endregion
    }
}