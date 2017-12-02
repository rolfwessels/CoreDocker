using System;
using CoreDocker.Utilities.FakeLogging;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoreDocker.Sdk.Tests
{
  public class SharedTestLogger
  {
    private static readonly Lazy<SharedTestLogger> _instance = new Lazy<SharedTestLogger>(() => new SharedTestLogger());

    private SharedTestLogger()
    {
      var loggerFactory = new LoggerFactory();
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.RollingFile(@"C:\temp\logs\CoreDocker.Tests.log", shared: true)
        .CreateLogger();

      loggerFactory.AddDebug();
      loggerFactory.AddSerilog();
      
      LogManager.SetLogger(loggerFactory);
      Log.Logger.Information("Starting logger");
    }

    #region singleton

    public static SharedTestLogger Instance => _instance.Value;

    #endregion


    public void EnsureEnabled()
    {
      //done;
    }
  }
}