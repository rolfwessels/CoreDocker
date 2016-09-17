using System.IO;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using log4net;

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
                .WriteTo.RollingFile(Path.Combine(@"C:\temp\logs", "CoreDocker.Sdk.Tests.log"))
                .CreateLogger();

            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            log4net.LogManager.SetLogger(loggerFactory);
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