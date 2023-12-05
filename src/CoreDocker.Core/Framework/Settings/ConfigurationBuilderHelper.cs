using Microsoft.Extensions.Configuration;

namespace CoreDocker.Core.Framework.Settings
{
    public static class ConfigurationBuilderHelper
    {
        public static IConfigurationBuilder AddJsonFilesAndEnvironment(this IConfigurationBuilder config,
            string environment = "Development")
        {
            var reloadOnChange = false;
            var optional = true;
            config.AddJsonFile("appsettings.json", optional, reloadOnChange)
                .AddJsonFile($"appsettings.{environment}.json", optional, reloadOnChange);
            config.AddEnvironmentVariables();
            return config;
        }
    }
}