using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace CoreDocker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
      

            System.Console.Title = "CoreDocker.Api";
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(args.FirstOrDefault()??"http://*:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
