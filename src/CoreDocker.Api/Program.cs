using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CoreDocker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "CoreDocker.Api";
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls(args.FirstOrDefault() ?? "http://*:5000")
                .ConfigureAppConfiguration(SettingsFileReaderHelper)
                .UseStartup<Startup>()
                .Build();
        }

        public static void SettingsFileReaderHelper(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            config.AddEnvironmentVariables();
        }
    }
}