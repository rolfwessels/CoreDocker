using CoreDocker.Core.Framework.Settings;
using Microsoft.Extensions.Configuration;

namespace CoreDocker.Api
{
    public class ApiSettings : BaseSettings
    {
        public ApiSettings(IConfiguration configuration) : base(configuration, "Api")
        {
        }

        //public string Origins => ReadConfigValue("Origins", "http://localhost:3000");
    }
}