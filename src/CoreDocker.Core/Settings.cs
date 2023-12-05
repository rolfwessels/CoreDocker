using System;
using System.Linq;
using Bumbershoot.Utilities;
using Microsoft.Extensions.Configuration;

namespace CoreDocker.Core
{
    public class Settings : BaseSettings
    {
        public Settings(IConfiguration configuration) : base(configuration, null)
        {
        }
        public string MongoConnection => ReadConfigValue("MongoConnection", "mongodb://localhost/CoreDocker-Sample");
        public string MongoDatabase => ReadConfigValue("MongoDatabase", new Uri(MongoConnection).Segments.FirstOrDefault(x => x != "/")??"CoreDocker");
        public string RedisHost => ReadConfigValue("RedisHost", "localhost:6390");
    }
}