using System;
using System.Linq;
using System.Reflection;
using CoreDocker.Core.Framework.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace CoreDocker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "CoreDocker.Api";
            
            Log.Logger = LoggingHelper.SetupOnce(() => new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File(@"c:\temp\logs\CoreDocker.Api.log", fileSizeLimitBytes: 10 * LoggingHelper.MB, rollOnFileSizeLimit:true)
                .WriteTo.Console(LogEventLevel.Information)
                //.ReadFrom.Configuration(BaseSettings.Config)
                .CreateLogger());

            try
            {
                BuildWebHost(args).Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((context, collection) =>
                        collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory()))
                .UseKestrel()
                .UseUrls(args.FirstOrDefault() ?? "http://*:5000")
                .ConfigureAppConfiguration(SettingsFileReaderHelper)
                .UseStartup<Startup>()
                .Build();
        }

        public static void SettingsFileReaderHelper(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment;
            config.AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
            config.AddEnvironmentVariables();
        }
    }
}
