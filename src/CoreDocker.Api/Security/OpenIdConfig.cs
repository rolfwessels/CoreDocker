using System.Collections.Generic;
using System.Security.Claims;
using CoreDocker.Core;
using CoreDocker.Core.Components.Users;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace CoreDocker.Console
{
    public class OpenIdConfig : OpenIdConfigBase
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("dataeventrecordsscope",
                    new[] {"role", "admin", "user", ResourceName, "dataEventRecords.admin", "dataEventRecords.user"}),
                new IdentityResource("securedfilesscope",
                    new[] {"role", "admin", "user", "securedFiles", "securedFiles.admin", "securedFiles.user"})
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(ResourceName)
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = ScopeApi,
                            DisplayName = "Standard api access"
                        }
                    },
                    UserClaims = {"role", "admin", "user"}
                },
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
                    AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("e0acca78-4dc2-46c6-83c6-c6aeacfffd46".Sha256())
                    },
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

        public static List<TestUser> Users()
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.GivenName, "admin"),
                new Claim(IdentityServerConstants.StandardScopes.Email, "admin@admin.com"),
                new Claim(JwtClaimTypes.Scope, OpenIdConfig.ScopeApi),
                new Claim(JwtClaimTypes.Role, "admin")
            };


      
            return new List<TestUser>()
            {
                new TestUser() {Username = "admin", Password = "admin!", SubjectId = "2", Claims = claims },
                new TestUser() {Username = "password", Password = "casd", SubjectId = "1",}
            };
        }
    }
}