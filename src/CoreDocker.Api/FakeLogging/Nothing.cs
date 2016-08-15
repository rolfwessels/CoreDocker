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

        internal static void SetLogger()
        {
        }

        internal static void SetLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory;
        }

        #region Nested type: LogAdaptor

        private class LogAdaptor : ILog
        {
            private ILogger _logger;

            public LogAdaptor(ILogger logger)
            {
                this._logger = logger;
            }

            public ILogger Logger {
                get { return _logger; } 
            }

            public void Error(string message)
            {
                _logger.LogError(message);
            }

            public void Error(string message, Exception exception)
            {
                _logger.LogError(message, exception);
            }

            public void Warn(string message)
            {
                _logger.LogWarning(message);
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

            public void Error(string message)
            {
            }

            public void Error(string message, Exception exception)
            {
            }

            public void Warn(string message)
            {
            }

            #endregion
        }

        #endregion
    }

    public interface ILog
    {
        ILogger Logger { get; }

        void Error(string error);
        void Error(string message, Exception exception);
        void Warn(string v);
    }
}