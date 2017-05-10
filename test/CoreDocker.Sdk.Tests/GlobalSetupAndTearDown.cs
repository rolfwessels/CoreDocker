using System.IO;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using log4net;
using Serilog.Sinks.RollingFileAlternate;

namespace CoreDocker.Sdk.Tests
{
    
    public class GlobalSetupAndTearDown
    {
        private static readonly ILog _log = LogManager.GetLogger<GlobalSetupAndTearDown>();

        [SetUp]
        public void GlobalSetup()
        {
            // Do login here.
            var loggerFactory = new LoggerFactory();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFileAlternate(@"C:\temp\logs\CoreDocker.Api", fileSizeLimitBytes: 1024  )
                .CreateLogger();

            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            LogManager.SetLogger(loggerFactory);
            Log.Logger.Information("Starting");
        }

        [TearDown]
        public void GlobalTeardown()
        {
            // Do logout here
            _log.Info("Dones");
        }
    }
}