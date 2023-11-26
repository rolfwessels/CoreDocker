using System;
using Bumbershoot.Utilities;
using Microsoft.Extensions.Configuration;

namespace CoreDocker.Core
{
    public class Settings : BaseSettings
    {
        private static Lazy<Settings> _instance = new(() => new Settings(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true).Build()));

        public Settings(IConfiguration configuration) : base(configuration, null)
        {
        }

        public static Settings Instance => _instance.Value;

        public string MongoConnection => ReadConfigValue("MongoConnection", "mongodb://localhost/");
        public string MongoDatabase => ReadConfigValue("MongoDatabase", "CoreDocker-Sample");
        public string WebBasePath => ReadConfigValue("WebBasePath", null);
        public string RedisHost => ReadConfigValue("RedisHost", "localhost:6390");

        public static void Initialize(IConfiguration configuration)
        {
            _instance = new Lazy<Settings>(() => new Settings(configuration));
        }
    }
}