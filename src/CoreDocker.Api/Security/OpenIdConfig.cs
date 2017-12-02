using System.Collections.Generic;
using IdentityServer4.Models;

namespace CoreDocker.Api.Security
{
    public class OpenIdConfig
    {
        public static string HostUrl = "https://localhost:44363";
        public const string ScopeApi = "Api";
        public const string CoredockerApi = "coredocker.api";

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("dataeventrecordsscope",new []{ "role", "admin", "user", "dataEventRecords", "dataEventRecords.admin" , "dataEventRecords.user" } ),
                new IdentityResource("securedfilesscope",new []{ "role", "admin", "user", "securedFiles", "securedFiles.admin", "securedFiles.user"} )
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("dataEventRecords")
                {
                    ApiSecrets =
                    {
                        new Secret("dataEventRecordsSecret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "dataeventrecordsscope",
                            DisplayName = "Scope for the dataEventRecords ApiResource"
                        }
                    },
                    UserClaims = { "role", "admin", "user", "dataEventRecords", "dataEventRecords.admin", "dataEventRecords.user" }
                },
                new ApiResource("securedFiles")
                {
                    ApiSecrets =
                    {
                        new Secret("securedFilesSecret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "securedfilesscope",
                            DisplayName = "Scope for the securedFiles ApiResource"
                        }
                    },
                    UserClaims = { "role", "admin", "user", "securedFiles", "securedFiles.admin", "securedFiles.user" }
                }
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientName = "CoreDocker Api",
                    ClientId = CoredockerApi,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        HostUrl
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        HostUrl + "/Unauthorized"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        HostUrl
                    },
                    AllowedScopes = new List<string>
                    {
                        ScopeApi
                    }
                }
            };
        }
    }
}