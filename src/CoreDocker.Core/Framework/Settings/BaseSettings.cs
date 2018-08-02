using Microsoft.Extensions.Configuration;

namespace CoreDocker.Core.Framework.Settings
{
    public class BaseSettings
    {
        private readonly IConfiguration _configuration;
        private readonly string _configGroup;

        public BaseSettings(IConfiguration configuration, string configGroup)
        {
            _configuration = configuration;
            _configGroup = configGroup;
        }

        protected string ReadConfigValue(string key, string defaultValue)
        {
            var section = string.IsNullOrWhiteSpace(_configGroup) ? _configuration : _configuration.GetSection(_configGroup);
            var value = section[key];
            return value ?? defaultValue;
        }
        
        protected void WriteConfigValue(string key, string value)
        {
            var section = string.IsNullOrWhiteSpace(_configGroup) ? _configuration : _configuration.GetSection(_configGroup);
            section[key] = value;
        }
    }
}