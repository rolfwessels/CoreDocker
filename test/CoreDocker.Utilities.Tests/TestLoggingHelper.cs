using CoreDocker.Core.Framework.Logging;
using Serilog;
using NUnit.Framework;
using Serilog.Events;

namespace CoreDocker.Utilities.Tests
{
    [SetUpFixture]
    public class TestLoggingHelper
    {
        static TestLoggingHelper()
        {
            EnsureExists();
        }

        public static void EnsureExists()
        {
            Log.Logger = LoggingHelper.SetupOnce(() => new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File(@"c:\temp\logs\CoreDocker.Api.Tests.log", fileSizeLimitBytes: 100000)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                //.ReadFrom.Configuration(BaseSettings.Config)
                .CreateLogger());
        }


        public TestLoggingHelper()
        {
            LoadRepo();
        }

        public static void LoadRepo()
        {
        }

        // [SetUp]
        public void ShowSomeTrace()
        {
        }
    }

    
}
