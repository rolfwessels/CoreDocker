using System;
using System.Linq;
using CoreDocker.Api.Security;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CoreDocker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "CoreDocker.Api";
            OpenIdConfigBase.HostUrl = "http://localhost:5000";
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls(args.FirstOrDefault() ?? "http://*:5000")
                .UseStartup<Startup>()
                .Build();
        }
    }
}