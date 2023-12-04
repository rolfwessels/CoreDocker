using Bumbershoot.Utilities;

namespace CoreDocker.Api.Security
{
    public class OpenIdSettings : BaseSettings
    {
        public OpenIdSettings(IConfiguration configuration) : base(configuration, "OpenId")
        {
        }

        public string HostUrl => ReadConfigValue("HostUrl", "http://localhost:5010");

        public string ApiResourceName => ReadConfigValue("ApiResourceName", "api.resource");

        public string ApiResourceSecret => ReadConfigValue("ApiResourceSecret", "a98802aa-e4d4-432a-835e-6c856a05d999");

        public string ClientName => ReadConfigValue("ClientName", "coredocker.api");

        public string ClientSecret => ReadConfigValue("ClientSecret", "super_secure_password");
        
        public string Origins =>
            ReadConfigValue("Origins", "http://localhost:5010,http://localhost:3000,http://localhost:84");

        public string CertPfx => ReadConfigValue("CertPfx", "development.pfx");

        public string CertPassword => ReadConfigValue("CertPassword", "60053018f4794862a82982640570c552");

        public string CertStoreThumbprint => ReadConfigValue("CertStoreThumbprint", "");

        public bool UseReferenceTokens => ReadConfigValue("UseReferenceTokens", true);

        public bool IsDebugEnabled => ReadConfigValue("IsDebugEnabled", true);

        public string[] GetOriginList()
        {
            return Origins.Split(',').ToArray();
        }
    }
}