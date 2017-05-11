using log4net;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests
{
  public class GlobalSetupAndTearDown
  {
    private static readonly ILog _log = LogManager.GetLogger<GlobalSetupAndTearDown>();

    [Test]
    public void method_GiventestingFor_Shouldresult()
    {
      SharedTestLogger.Instance.EnsureEnabled();
    }
  }
}