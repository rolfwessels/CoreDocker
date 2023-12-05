using CoreDocker.Core.Framework.Logging;
using Serilog.Events;
using Path = System.IO.Path;

namespace CoreDocker.Api;

public static class Logging
{
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = LoggingHelper.SetupOnce(() => new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(Path.GetTempPath(), "logs", "CoreDocker.Api.log"),
                fileSizeLimitBytes: 10 * LoggingHelper.MB,
                rollOnFileSizeLimit: true)
            .CreateLogger());
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);
    }
}