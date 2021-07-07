using System;
using System.Reflection;
using CoreDocker.Core.Framework.Logging;
using Serilog;
using NUnit.Framework;
using Serilog.Events;

namespace CoreDocker.Utilities.Tests
{
    [SetUpFixture]
    public class TestLoggingHelper
    {
        private static Lazy<ILogger> _logger;

        static TestLoggingHelper()
        {
            _logger = new Lazy<ILogger>(SetupOnce);
        }

        public static void EnsureExists()
        {
            Log.Logger = _logger.Value;
        }

        private static ILogger SetupOnce()
        {
            return LoggingHelper.SetupOnce(() => new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.Console(LogEventLevel.Debug)
                .CreateLogger());
        }
    }
}