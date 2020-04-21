﻿using System.Linq;
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

        public string Origins => ReadConfigValue("Origins", "http://localhost:5000");

        public string CertPfx => ReadConfigValue("CertPfx", "development.pfx");

        public string CertPassword => ReadConfigValue("CertPassword", "60053018f4794862a82982640570c552");

        public string CertStoreThumbprint => ReadConfigValue("certStoreThumbprint", "B75303B3E5CEBE484C342D438987AB33560B5717");
        //B75303B3E5CEBE484C342D438987AB33560B5717


        public string[] GetOriginList()
        {
            return Origins.Split(',').ToArray();
        }
    }
}