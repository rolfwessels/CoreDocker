using System.Diagnostics;
using log4net;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests
{
  [SetUpFixture]
  public class GlobalSetupAndTearDown
  {
    private static readonly ILog _log = LogManager.GetLogger<GlobalSetupAndTearDown>();

    public GlobalSetupAndTearDown()
    {
      SharedTestLogger.Instance.EnsureEnabled();
      _log.Info("Test");
      LogManager.GetLogger<GlobalSetupAndTearDown>().Info("te");
    }

   // [SetUp]
    public void ShowSomeTrace()
    {
      
    }

  }
}