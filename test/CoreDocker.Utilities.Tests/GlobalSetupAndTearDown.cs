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
        static GlobalSetupAndTearDown()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("logSettings.xml"));
        }

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GlobalSetupAndTearDown()
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