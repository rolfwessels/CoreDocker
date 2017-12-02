using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace CoreDocker.Utilities.Tests
{
    [SetUpFixture]
    public class GlobalSetupAndTearDown
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GlobalSetupAndTearDown()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("logSettings.xml"));
        }

        // [SetUp]
        public void ShowSomeTrace()
        {
        }
    }
}