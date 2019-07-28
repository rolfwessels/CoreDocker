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
        public static void EnsureExists()
        {
            Log.Logger = LoggingHelper.SetupOnce(() => new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File(@"c:\temp\logs\CoreDocker.Api.Tests.log")
                .WriteTo.Console(LogEventLevel.Debug)
                //.ReadFrom.Configuration(BaseSettings.Config)
                .CreateLogger());
        }
    }

    
}
