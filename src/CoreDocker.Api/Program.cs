using System.IO;
using System.Linq;
using CoreDocker.Core;
using Microsoft.AspNetCore.Hosting;

namespace CoreDocker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            OpenIdConfigBase.HostUrl = "http://localhost:5000";
            System.Console.Title = "CoreDocker.Api";
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(args.FirstOrDefault()??"http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
