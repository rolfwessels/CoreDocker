using System;
using Microsoft.Extensions.Configuration;


namespace CoreDocker.Utilities
{
    public class Settings
    {
        private readonly IConfigurationRoot _configuration;

        private static Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());

        private Settings(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        private Settings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);
            _configuration = builder.Build();
        }

        #region singleton

        public static Settings Instance => _instance.Value;

        #endregion

        public string MongoConnection => _configuration["MongoConnection"] ?? "mongodb://localhost/";
        public string MongoDatabase => _configuration["MongoDatabase"]?? "CoreDocker-Sample";
        public string WebBasePath => _configuration["WebBasePath"] ;
        
        public static void Initialize(IConfigurationRoot configuration)
        {
            _instance = new Lazy<Settings>(() => new Settings(configuration));
        }
    }
}