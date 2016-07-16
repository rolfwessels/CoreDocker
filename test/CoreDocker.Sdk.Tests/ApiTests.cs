using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using CoreDocker.Api;

namespace CoreDocker.Sdk.Tests
{
    [TestFixture]
    public class ApiTests
    {
        [Test]
        public void Get_CallsConfig_ShouldReturnConfigValues()
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

            var host = new WebHostBuilder()
//                .UseKestrel()
//                .UseContentRoot(Directory.GetCurrentDirectory())
//                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();

            ILogger logger = loggerFactory.CreateLogger<ApiTests>();
            FlurlHelper.Log = (t) => logger.LogInformation(t);
            var connectionFactory = new ConnectionFactory("http://localhost:5000/");
            var connection = connectionFactory.GetConnection();
            connection.Configs.Get().Wait();
        }
    }
}