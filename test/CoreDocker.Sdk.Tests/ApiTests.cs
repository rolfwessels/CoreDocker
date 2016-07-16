using System.Runtime.InteropServices;
using NUnit.Framework;
using CoreDocker.Sdk;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Sdk.Tests
{
    [TestFixture]
    public class ApiTests
    {
        
        [Test]
        public void CanAddNumbers()
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();
            ILogger logger = loggerFactory.CreateLogger<ApiTests>();
            FlurlHelper.Log = (t) => logger.LogInformation(t);
            var connectionFactory = new ConnectionFactory("http://localhost:5000/");
            var connection = connectionFactory.GetConnection();
            connection.Configs.Get().Wait();
        }
    }
}
