using CoreDocker.Core.Framework.Settings;
using Microsoft.Extensions.Configuration;

namespace CoreDocker.Api.Security
{
    public class OpenIdSettings : BaseSettings
    {
        public OpenIdSettings(IConfiguration configuration) : base(configuration, "OpenId")
        {
        }

        public string HostUrl => ReadConfigValue("HostUrl", "http://localhost:5000");


        public string ApiResourceName => ReadConfigValue("ApiResourceName", "api.resource");

        public string ApiResourceSecret => ReadConfigValue("ApiResourceSecret", "a98802aa-e4d4-432a-835e-6c856a05d999");

        public string ClientName => ReadConfigValue("ClientName", "coredocker.api");

        public string ClientSecret => ReadConfigValue("ClientSecret", "super_secure_password");

        public string IdentPath => ReadConfigValue("IdentPath", "identity");

        public string ScopeApi => ReadConfigValue("ScopeApi", "api");
    }
}